using ErrorOr;
using PicoNet.Contracts.DTOs.Requests.Roles;
using PicoNet.Contracts.DTOs.Requests.Users;
using PicoNet.Contracts.DTOs.Responses.Roles;
using PicoNet.Contracts.DTOs.Responses.Users;
using PicoNet.UI.ApiClients.Interfaces;

namespace PicoNet.UI.ApiClients.Implementations;

public sealed class AdminApiClient : IAdminApiClient
{
    private readonly HttpClient _http;

    public AdminApiClient(HttpClient http) => _http = http;

    #region Users

    public async Task<ErrorOr<ListUsersResponse>> ListUsersAsync(int pageNumber = 1, int pageSize = 10, CancellationToken ct = default)
    {
        var url = $"/api/users?pageNumber={pageNumber}&pageSize={pageSize}";
        var response = await _http.GetAsync(url, ct);

        if (!response.IsSuccessStatusCode)
            return await response.ToErrorListAsync(ct);

        return await response.Content.ReadFromJsonAsync<ListUsersResponse>(ct) ??
               (ErrorOr<ListUsersResponse>)Error.Unexpected("Admin.Unexpected", "Server returned an empty response.");
    }

    public async Task<ErrorOr<UserResponse>> GetUserAsync(Guid userId, CancellationToken ct = default)
    {
        var response = await _http.GetAsync($"/api/users/{userId}", ct);

        if (!response.IsSuccessStatusCode)
            return await response.ToErrorListAsync(ct);

        return await response.Content.ReadFromJsonAsync<UserResponse>(ct) ??
               (ErrorOr<UserResponse>)Error.Unexpected("Admin.Unexpected", "Server returned an empty response.");
    }

    public async Task<ErrorOr<UserResponse>> CreateUserAsync(CreateUserRequest request, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync("/api/users", request, ct);

        if (!response.IsSuccessStatusCode)
            return await response.ToErrorListAsync(ct);

        return await response.Content.ReadFromJsonAsync<UserResponse>(ct) ??
               (ErrorOr<UserResponse>)Error.Unexpected("Admin.Unexpected", "Server returned an empty response.");
    }

    public async Task<ErrorOr<UserResponse>> UpdateUserAsync(Guid userId, UpdateUserRequest request, CancellationToken ct = default)
    {
        var response = await _http.PatchAsJsonAsync($"/api/users/{userId}", request, ct);

        if (!response.IsSuccessStatusCode)
            return await response.ToErrorListAsync(ct);

        return await response.Content.ReadFromJsonAsync<UserResponse>(ct) ??
               (ErrorOr<UserResponse>)Error.Unexpected("Admin.Unexpected", "Server returned an empty response.");
    }

    public async Task<ErrorOr<bool>> DeleteUserAsync(Guid userId, CancellationToken ct = default)
    {
        var response = await _http.DeleteAsync($"/api/users/{userId}", ct);

        if (!response.IsSuccessStatusCode)
            return await response.ToErrorListAsync(ct);

        return true;
    }

    public async Task<ErrorOr<UserResponse>> UpdateUserRolesAsync(Guid userId, List<string> roles, CancellationToken ct = default)
    {
        // Get current user roles
        var userResult = await GetUserAsync(userId, ct);
        if (userResult.IsError)
            return userResult.Errors;

        var currentUser = userResult.Value;
        var currentRoles = currentUser.Roles;

        // Determine roles to add and remove
        var rolesToAdd = roles.Except(currentRoles).ToList();
        var rolesToRemove = currentRoles.Except(roles).ToList();

        // Remove roles
        foreach (var roleToRemove in rolesToRemove)
        {
            var removeResponse = await _http.DeleteAsync($"/api/users/{userId}/roles/{Uri.EscapeDataString(roleToRemove)}", ct);
            if (!removeResponse.IsSuccessStatusCode)
                return await removeResponse.ToErrorListAsync(ct);
        }

        // Add roles
        foreach (var roleToAdd in rolesToAdd)
        {
            var addRequest = new ManageRoleRequest { RoleName = roleToAdd };
            var addResponse = await _http.PostAsJsonAsync($"/api/users/{userId}/roles", addRequest, ct);
            if (!addResponse.IsSuccessStatusCode)
                return await addResponse.ToErrorListAsync(ct);
        }

        // Return updated user
        return await GetUserAsync(userId, ct);
    }

    public async Task<ErrorOr<bool>> ResetPasswordAsync(Guid userId, CancellationToken ct = default)
    {
        var tempPassword = GenerateTemporaryPassword();
        var request = new ForcePasswordChangeRequest
        {
            NewPassword = tempPassword,
            SendEmailNotification = true
        };

        var response = await _http.PostAsJsonAsync($"/api/users/{userId}/force-password-change", request, ct);

        if (!response.IsSuccessStatusCode)
            return await response.ToErrorListAsync(ct);

        return true;
    }

    #endregion

    #region Roles

    public async Task<ErrorOr<List<RoleResponse>>> ListRolesAsync(CancellationToken ct = default)
    {
        var response = await _http.GetAsync("/api/roles", ct);

        if (!response.IsSuccessStatusCode)
            return await response.ToErrorListAsync(ct);

        return await response.Content.ReadFromJsonAsync<List<RoleResponse>>(ct) ??
               (ErrorOr<List<RoleResponse>>)Error.Unexpected("Admin.Unexpected", "Server returned an empty response.");
    }

    public async Task<ErrorOr<RoleResponse>> CreateRoleAsync(CreateRoleRequest request, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync("/api/roles", request, ct);

        if (!response.IsSuccessStatusCode)
            return await response.ToErrorListAsync(ct);

        return await response.Content.ReadFromJsonAsync<RoleResponse>(ct) ??
               (ErrorOr<RoleResponse>)Error.Unexpected("Admin.Unexpected", "Server returned an empty response.");
    }

    public async Task<ErrorOr<bool>> DeleteRoleAsync(string roleName, CancellationToken ct = default)
    {
        var response = await _http.DeleteAsync($"/api/roles/{Uri.EscapeDataString(roleName)}", ct);

        if (!response.IsSuccessStatusCode)
            return await response.ToErrorListAsync(ct);

        return true;
    }

    #endregion

    private static string GenerateTemporaryPassword()
    {
        // Generate a complex temporary password: at least 8 chars with uppercase, lowercase, digit, and special char
        const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lower = "abcdefghijklmnopqrstuvwxyz";
        const string digits = "0123456789";
        const string special = "!@#$%^&*";

        var rnd = new Random();
        var password = new System.Text.StringBuilder();

        password.Append(upper[rnd.Next(upper.Length)]);
        password.Append(lower[rnd.Next(lower.Length)]);
        password.Append(digits[rnd.Next(digits.Length)]);
        password.Append(special[rnd.Next(special.Length)]);

        for (int i = password.Length; i < 12; i++)
        {
            var all = upper + lower + digits + special;
            password.Append(all[rnd.Next(all.Length)]);
        }

        var result = password.ToString().ToCharArray();
        Random.Shared.Shuffle(result);
        return new string(result);
    }
}
