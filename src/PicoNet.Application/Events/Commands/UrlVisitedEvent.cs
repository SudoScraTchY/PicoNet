using PicoNet.Application.Features.Redirect.Commands;
using PicoNet.Contracts.DTOs.Cache;

namespace PicoNet.Application.Events.Commands;

public record UrlVisitedEvent(
    Guid UrlId,
    bool FromCache,
    CachedRedirectDto CachedRedirect,
    RedirectCommand RedirectCommand,
    DateTime VisitedAt);