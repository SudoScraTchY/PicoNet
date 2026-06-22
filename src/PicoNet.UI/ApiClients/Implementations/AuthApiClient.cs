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

        if (!response.IsSuccessStatusCode)
            return await response.ToErrorListAsync(ct);

        return await response.Content.ReadFromJsonAsync<ErrorOr<AuthResponse>>(cancellationToken: ct);
    }
    
    public async Task<ErrorOr<RegisterResponse>> RegisterAsync(RegisterRequest command, CancellationToken ct)
    {
        var response = await _http.PostAsJsonAsync("/api/auth/register", command, ct);
        
        if (!response.IsSuccessStatusCode)
            return await response.ToErrorListAsync(ct);
        
        return await response.Content.ReadFromJsonAsync<ErrorOr<RegisterResponse>>(cancellationToken: ct);
    }

    public async Task<ErrorOr<AuthResponse>> ValidateEmailAsync(ValidateRegistrationRequest command, CancellationToken ct)
    {
        var response = await _http.PostAsJsonAsync("/api/auth/validate", command, ct);
        
        if (!response.IsSuccessStatusCode)
            return await response.ToErrorListAsync(ct);
        
        return await response.Content.ReadFromJsonAsync<ErrorOr<AuthResponse>>(cancellationToken: ct);
    }
}