using System.Net.Http.Headers;

namespace PicoNet.UI.Services;

public sealed class AuthHeaderHandler : DelegatingHandler
{
    private readonly CurrentUserTokenAccessor _tokenAccessor;

    public AuthHeaderHandler(CurrentUserTokenAccessor tokenAccessor) => _tokenAccessor = tokenAccessor;

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken ct)
    {
        if (!string.IsNullOrEmpty(_tokenAccessor.AccessToken))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenAccessor.AccessToken);
        }
        return base.SendAsync(request, ct);
    }
}