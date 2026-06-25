using System.Net;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using PicoNet.Contracts.DTOs.Requests.Auth;
using PicoNet.UI.ApiClients.Interfaces;

namespace PicoNet.UI.ApiClients.Implementations;

public class BlazorInternalApi : IBlazorInternalApi
{
    private readonly HttpClient _httpClient;

    public BlazorInternalApi(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ErrorOr<HttpResponseMessage>> Login(LoginRequest model)
    { 
        var response = await _httpClient.PostAsJsonAsync("/api/auth/login", model);
    
        if (!response.IsSuccessStatusCode)
        {
            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            // handle errors
        }
        return  response;
    }
}