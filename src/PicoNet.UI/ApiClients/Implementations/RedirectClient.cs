using ErrorOr;
using PicoNet.Contracts.DTOs.Responses.Redirect;
using PicoNet.UI.ApiClients.Interfaces;

namespace PicoNet.UI.ApiClients.Implementations;

public class RedirectClient : IRedirectClient
{
    private readonly HttpClient _http;

    public RedirectClient(HttpClient http) => _http = http;

    public async Task<ErrorOr<RedirectUrlResult?>> ResolveAsync(string shortCode,string? password = null)
    {
        
        if (shortCode == "fail") 
            return Error.NotFound();
        
        if (shortCode == "success") 
            return new RedirectUrlResult("www.google.com")
            {
                IsPasswordProtected = true,
                MaxClicks = 10
            };
        
        return shortCode == "HasPassword" ? Error.Unauthorized() : Error.Forbidden();


        // var response =
        //     await _http.GetAsync(
        //         $"api/redirect/{shortCode}");
        //
        // if (!response.IsSuccessStatusCode)
        //     return Error.NotFound();
        //
        // var result = await response.Content.ReadFromJsonAsync<RedirectUrlResult>() ?? null;
        // return result;
    }
}