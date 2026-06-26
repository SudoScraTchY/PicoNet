using BitzArt.Blazor.Auth.Server;
using PicoNet.Contracts.DTOs.Requests.Auth;
using BitzArt.Blazor.Auth;
using PicoNet.UI.ApiClients.Interfaces;

namespace PicoNet.UI.Services;

public class PicoNetAuthenticationService : AuthenticationService<LoginRequest>
{
    private readonly IAuthApiClient _authClient;
    private readonly ICircuitTokenStore _tokenStore;

    public PicoNetAuthenticationService(IAuthApiClient authClient, ICircuitTokenStore circuitTokenStore)
    {
        _authClient = authClient;
        _tokenStore = circuitTokenStore;
    }

    public override async Task<AuthenticationResult> SignInAsync(
        LoginRequest payload, CancellationToken cancellationToken = default)
    {
        // Call your custom API
        var result = await _authClient.LoginAsync(
            new LoginRequest { Email = payload.Email, Password = payload.Password }, 
            cancellationToken);

        if (result.IsError)
        {
            // Map your errors to Blazor.Auth's failure format
            return Failure(result.Errors.First().Description);
        }

        var tokens = result.Value.Tokens;
        _tokenStore.AccessToken = tokens.AccessToken;
        _tokenStore.RefreshToken = tokens.RefreshToken;
        _tokenStore.AccessTokenExpiresAt = tokens.AccessExpiresAt;
        _tokenStore.RefreshTokenExpiresAt = tokens.RefreshExpiresAt;

        return Success(new JwtPair(
            tokens.AccessToken, tokens.AccessExpiresAt, tokens.RefreshToken, tokens.RefreshExpiresAt));
    }

    public override async Task<AuthenticationResult> RefreshJwtPairAsync(
        string refreshToken, 
        CancellationToken cancellationToken = default)
    {
        // Call your API's refresh endpoint
        // You'll need to add a RefreshToken method to IAuthApiClient
        var result = await _authClient.RefreshTokenAsync(refreshToken, cancellationToken);

        if (result.IsError)
        {
            return Failure(result.Errors.First().Description);
        }

        var tokens = result.Value;
        _tokenStore.AccessToken = tokens.AccessToken;
        _tokenStore.RefreshToken = tokens.RefreshToken;
        _tokenStore.AccessTokenExpiresAt = tokens.AccessExpiresAt;
        _tokenStore.RefreshTokenExpiresAt = tokens.RefreshExpiresAt;

        return Success(new JwtPair(
            tokens.AccessToken, tokens.AccessExpiresAt, tokens.RefreshToken, tokens.RefreshExpiresAt));
    }
}