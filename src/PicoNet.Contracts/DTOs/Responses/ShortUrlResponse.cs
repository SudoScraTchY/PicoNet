namespace PicoNet.Contracts.DTOs.Responses;

public record ShortUrlResponse(
    Guid Id,
    string ShortCode,
    string OriginalUrl,
    string? CustomAlias,
    DateTime CreatedAt,
    DateTime? ExpiresAt,
    bool IsActive,
    List<string>? Tags,
    int VisitCount = 0
);