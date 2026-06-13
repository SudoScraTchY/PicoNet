namespace PicoNet.Application.Events.Commands;

public record UrlVisitedEvent(
    Guid UrlId,
    string ShortCode,
    bool FromCache,
    string OriginalUrl,
    string? PasswordHash,
    long ClickCount,
    int? MaxClicks,
    string Status,
    DateTime? ExpiryTime,
    string? IpAddress,
    string? UserAgent,
    string? Referrer,
    DateTime VisitedAt
);