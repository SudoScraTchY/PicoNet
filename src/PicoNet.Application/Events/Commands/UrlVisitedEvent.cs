namespace PicoNet.Application.Events.Commands;

public record UrlVisitedEvent(
    Guid UrlId,
    string ShortCode,
    string? IpAddress,
    string? UserAgent,
    string? Referrer,
    DateTime VisitedAt);