using PicoNet.Domain.Enums;

namespace PicoNet.Contracts.DTOs.Requests.Shortner;

public record EditShortUrlRequest(
    Guid ShortenedUrlId,
    string? OriginalUrl,
    string? CustomAlias,
    UrlStatus? UrlStatus,
    List<string>? Tags,
    bool? IsPermanent,
    DateTime? ExpiryTime,
    string? Password,
    string?Campaign);