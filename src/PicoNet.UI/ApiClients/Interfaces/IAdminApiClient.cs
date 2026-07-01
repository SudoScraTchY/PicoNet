using ErrorOr;
using PicoNet.Contracts.DTOs.Requests.Roles;
using PicoNet.Contracts.DTOs.Requests.Users;
using PicoNet.Contracts.DTOs.Responses.Roles;
using PicoNet.Contracts.DTOs.Responses.Users;

namespace PicoNet.UI.ApiClients.Interfaces;

public interface IAdminApiClient
{
    #region Users
    Task<ErrorOr<ListUsersResponse>> ListUsersAsync(int pageNumber = 1, int pageSize = 10, CancellationToken ct = default);
    Task<ErrorOr<UserResponse>> GetUserAsync(Guid userId, CancellationToken ct = default);
    Task<ErrorOr<UserResponse>> CreateUserAsync(CreateUserRequest request, CancellationToken ct = default);
    Task<ErrorOr<UserResponse>> UpdateUserAsync(Guid userId, UpdateUserRequest request, CancellationToken ct = default);
    Task<ErrorOr<bool>> DeleteUserAsync(Guid userId, CancellationToken ct = default);
    Task<ErrorOr<UserResponse>> UpdateUserRolesAsync(Guid userId, List<string> roles, CancellationToken ct = default);
    Task<ErrorOr<bool>> ResetPasswordAsync(Guid userId, CancellationToken ct = default);
    #endregion

    #region Roles
    Task<ErrorOr<List<RoleResponse>>> ListRolesAsync(CancellationToken ct = default);
    Task<ErrorOr<RoleResponse>> CreateRoleAsync(CreateRoleRequest request, CancellationToken ct = default);
    Task<ErrorOr<bool>> DeleteRoleAsync(string roleName, CancellationToken ct = default);
    #endregion
}
