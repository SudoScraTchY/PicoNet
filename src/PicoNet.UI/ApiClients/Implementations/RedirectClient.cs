using ErrorOr;
using PicoNet.Contracts.DTOs.Responses.Redirect;
using PicoNet.UI.ApiClients.Interfaces;

namespace PicoNet.UI.ApiClients.Implementations;

public class RedirectClient : IRedirectClient
{
    private readonly HttpClient _http;

    public RedirectClient(HttpClient http) => _http = http;

    public async Task<ErrorOr<RedirectUrlResult?>> ResolveAsync(string shortCode)
    {
        var response =
            await _http.GetAsync(
                $"api/redirect/{shortCode}");

        if (!response.IsSuccessStatusCode)
            return Error.NotFound();

        var result = await response.Content.ReadFromJsonAsync<RedirectUrlResult>() ?? null;
        return result;
    }
}