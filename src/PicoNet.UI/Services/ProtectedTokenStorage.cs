using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace PicoNet.UI.Services;

public class ProtectedTokenStorage : ITokenStorage
{
    private readonly ProtectedLocalStorage _localStorage;
    private const string AccessTokenKey = "access_token";
    private const string RefreshTokenKey = "refresh_token";

    public ProtectedTokenStorage(ProtectedLocalStorage localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task SaveTokensAsync(string accessToken, string refreshToken)
    {
        await _localStorage.SetAsync(AccessTokenKey, accessToken);
        await _localStorage.SetAsync(RefreshTokenKey, refreshToken);
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        var result = await _localStorage.GetAsync<string>(AccessTokenKey);
        return result.Success ? result.Value : null;
    }

    public async Task<string?> GetRefreshTokenAsync()
    {
        var result = await _localStorage.GetAsync<string>(RefreshTokenKey);
        return result.Success ? result.Value : null;
    }

    public async Task ClearTokensAsync()
    {
        await _localStorage.DeleteAsync(AccessTokenKey);
        await _localStorage.DeleteAsync(RefreshTokenKey);
    }
}