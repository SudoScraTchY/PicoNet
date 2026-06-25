using Microsoft.JSInterop;
namespace PicoNet.UI.Services;
public interface IUserTokenProvider
{
    Task<string?> GetAccessTokenAsync();
    Task SetAccessTokenAsync(string token);
    Task RemoveAccessTokenAsync();
}
public sealed class UserTokenProvider : IUserTokenProvider
{
    private const string AccessTokenKey = "access_token";
    private readonly IJSRuntime _js;

    public UserTokenProvider(IJSRuntime js)
    {
        _js = js;
    }

    public async Task<string?> GetAccessTokenAsync()
        => await _js.InvokeAsync<string?>("localStorage.getItem", AccessTokenKey);
    public async Task SetAccessTokenAsync(string token)
        => await _js.InvokeVoidAsync("localStorage.setItem", AccessTokenKey, token);
    public async Task RemoveAccessTokenAsync()
        => await _js.InvokeVoidAsync("localStorage.removeItem", AccessTokenKey);
}