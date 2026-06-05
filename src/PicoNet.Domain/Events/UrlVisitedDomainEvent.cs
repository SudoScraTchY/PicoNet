using PicoNet.Domain.Entities.Common.Concrete;

namespace PicoNet.Domain.Events;

public record UrlVisitedDomainEvent(
    Guid UrlId,
    string ShortCode,
    string? IpAddress,
    string? UserAgent,
    string? Referrer,
    DateTime VisitedAt) : DomainEvent;