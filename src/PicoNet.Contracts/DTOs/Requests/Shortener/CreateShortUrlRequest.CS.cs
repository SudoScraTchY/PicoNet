namespace PicoNet.Contracts.DTOs.Requests.Shortener;

public record CreateShortUrlRequest(
    string OriginalUrl,
    string? CustomAlias = null,
    List<string>? Tags = null
);