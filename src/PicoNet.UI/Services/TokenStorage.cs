using PicoNet.Contracts.DTOs.Responses.Auth;

namespace PicoNet.UI.Services;

public interface ITokenStorage
{
    Task SaveTokensAsync(string accessToken, string refreshToken);
    Task<string?> GetAccessTokenAsync();
    Task<string?> GetRefreshTokenAsync();
    Task ClearTokensAsync();
}