using ErrorOr;
using Microsoft.AspNetCore.Identity.Data;
using PicoNet.Contracts.DTOs.Requests.Auth;
using PicoNet.Contracts.DTOs.Responses.Auth;
using PicoNet.UI.ApiClients.Interfaces;
using LoginRequest = PicoNet.Contracts.DTOs.Requests.Auth.LoginRequest;
using RegisterRequest = PicoNet.Contracts.DTOs.Requests.Auth.RegisterRequest;

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

        return await response.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken: ct) 
               ?? (ErrorOr<AuthResponse>)Error.Unexpected("Auth.EmptyResponse", "Server returned an empty response.");
    }
    
    public async Task<ErrorOr<AuthTokenResponse>> RefreshTokenAsync(string refreshToken, CancellationToken ct)
    {
        var response = await _http.GetAsync("auth/refresh", cancellationToken: ct);
        
        if (!response.IsSuccessStatusCode)
            return await response.ToErrorListAsync(ct);
        
        return await response.Content.ReadFromJsonAsync<AuthTokenResponse>(cancellationToken: ct) 
               ?? (ErrorOr<AuthTokenResponse>)Error.Unexpected("Auth.EmptyResponse", "Server returned an empty response.");
            
    }
    
    public async Task<ErrorOr<AuthResponse>> RegisterAsync(RegisterRequest command, CancellationToken ct)
    {
        var response = await _http.PostAsJsonAsync("/api/auth/register", command, ct);
        
        if (!response.IsSuccessStatusCode)
            return await response.ToErrorListAsync(ct);
        
        return await response.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken: ct) 
               ?? (ErrorOr<AuthResponse>)Error.Unexpected("Auth.EmptyResponse", "Server returned an empty response.");
    }

    public async Task<ErrorOr<AuthResponse>> ConfirmEmailAsync(ValidateRegistrationRequest command, CancellationToken ct)
    {
        var response = await _http.PostAsJsonAsync("/api/auth/confirm-email", command, ct);
        
        if (!response.IsSuccessStatusCode)
            return await response.ToErrorListAsync(ct);
        
        return await response.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken: ct) 
               ?? (ErrorOr<AuthResponse>)Error.Unexpected("Auth.EmptyResponse", "Server returned an empty response.");
    }

    public async Task<ErrorOr<ChangeEmailResponse>> ChangeEmailAsync(ChangeEmailRequest command, CancellationToken ct)
    {
        var response = await _http.PatchAsJsonAsync("/api/auth/ChangeEmail", command, ct);
        
        if (!response.IsSuccessStatusCode)
            return await response.ToErrorListAsync(ct);
        
        return await response.Content.ReadFromJsonAsync<ChangeEmailResponse>(cancellationToken: ct) 
               ?? (ErrorOr<ChangeEmailResponse>)Error.Unexpected("Auth.EmptyResponse", "Server returned an empty response.");
    }
}