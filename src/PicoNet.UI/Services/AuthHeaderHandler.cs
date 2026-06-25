using System.Net.Http.Headers;

namespace PicoNet.UI.Services;

public sealed class AuthHeaderHandler : DelegatingHandler
{
    private readonly ITokenStorage _tokenStorage;
    private readonly CurrentUserTokenAccessor _currentUserTokenAccessor;

    public AuthHeaderHandler(ITokenStorage tokenStorage, CurrentUserTokenAccessor currentUserTokenAccessor)
    {
        _tokenStorage = tokenStorage;
        _currentUserTokenAccessor = currentUserTokenAccessor;
    }

// Inside your AuthHeaderHandler.cs / AuthHeaderHandler class
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var path = request.RequestUri?.AbsolutePath ?? "";
        var token1 = _currentUserTokenAccessor.AccessToken;

        // 1. Bypass token extraction completely for anonymous auth endpoints
        if (path.Contains("/login", StringComparison.OrdinalIgnoreCase) || 
            path.Contains("/register", StringComparison.OrdinalIgnoreCase))
        {
            return await base.SendAsync(request, cancellationToken);
        }

        try
        {
            var token = await _tokenStorage.GetAccessTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token1);
            }
        }
        catch (InvalidOperationException exception)
        {
            // 2. Safe fallback: If a non-auth endpoint is hit during an unexpected SSR state,
            // log it or skip attaching the header instead of crashing the SignalR circuit.
        }

        return await base.SendAsync(request, cancellationToken);
    }
}