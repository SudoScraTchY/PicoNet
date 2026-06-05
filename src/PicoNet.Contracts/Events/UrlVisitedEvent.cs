namespace PicoNet.Contracts.Events;

public record UrlVisitedEvent(
    Guid UrlId,
    string ShortCode,
    string? IpAddress,
    string? UserAgent,
    string? Referrer,
    DateTime VisitedAt);