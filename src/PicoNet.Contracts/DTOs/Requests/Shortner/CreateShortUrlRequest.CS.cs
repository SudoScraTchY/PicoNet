namespace PicoNet.Contracts.DTOs.Requests;

public record CreateShortUrlRequest(
    string OriginalUrl,
    string? CustomAlias = null,
    List<string>? Tags = null
);