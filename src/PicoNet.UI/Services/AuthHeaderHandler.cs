namespace PicoNet.UI.Services;

using System.Net.Http.Headers;

public sealed class AuthHeaderHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor? _httpContextAccessor;
    private readonly ICircuitTokenStore? _circuitTokenStore;
    private readonly ILogger<AuthHeaderHandler> _logger;

    public AuthHeaderHandler(
        IHttpContextAccessor? httpContextAccessor,
        ILogger<AuthHeaderHandler> logger, ICircuitTokenStore? circuitTokenStore)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _circuitTokenStore = circuitTokenStore;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor?.HttpContext;
        
        if (httpContext != null)
        {
            // Read cookies that BitzArt set
            if (httpContext.Request.Cookies.TryGetValue("AccessToken", out var accessToken) 
                && !string.IsNullOrEmpty(accessToken))
            {
                request.Headers.Authorization = 
                    new AuthenticationHeaderValue("Bearer", accessToken);
            }

            if (httpContext.Request.Cookies.TryGetValue("RefreshToken", out var refreshToken) 
                && !string.IsNullOrEmpty(refreshToken))
            {
                request.Headers.TryAddWithoutValidation("RefreshToken", refreshToken);
            }
        }
        else
        {
            _logger.LogWarning("HttpContext is null — cannot attach auth token");
        }

        return base.SendAsync(request, cancellationToken);
    }
}
