using ErrorOr;
using PicoNet.Contracts.DTOs.Requests.Auth;
using PicoNet.Contracts.DTOs.Responses.Auth;
using PicoNet.UI.ApiClients.Interfaces;

namespace PicoNet.UI.ApiClients.Implementations;

public class AuthApiClient : IAuthApiClient
{
    private readonly HttpClient _http;

    public AuthApiClient(HttpClient http) => _http = http;
    
    public async Task<ErrorOr<AuthResponse>> LoginAsync(LoginRequest command, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync("/api/auth/login", command, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken: ct) ?? new ErrorOr<AuthResponse>();
    }

    public async Task<ErrorOr<RegisterResponse>> RegisterAsync(RegisterRequest command, CancellationToken ct)
    {
        var response = await _http.PostAsJsonAsync("/api/auth/register", command, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<RegisterResponse>(cancellationToken: ct) ?? new ErrorOr<RegisterResponse>();
    }

    public async Task<ErrorOr<AuthResponse>> ValidateEmailAsync(ValidateRegistrationRequest command, CancellationToken ct)
    {
        var response = await _http.PostAsJsonAsync("/api/auth/validate", command, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken: ct) ?? new ErrorOr<AuthResponse>();
    }
}