namespace PicoNet.UI.Services;

public class TokenForwardingHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TokenForwardingHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        
        if (httpContext != null)
        {
            // Forward the cookies from the browser's request to the API
            if (httpContext.Request.Cookies.TryGetValue("Access_Token", out var accessToken))
            {
                request.Headers.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            }
            
            // Forward refresh token if needed
            if (httpContext.Request.Cookies.TryGetValue("Refresh_Token", out var refreshToken))
            {
                request.Headers.TryAddWithoutValidation("X-Refresh-Token", refreshToken);
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}