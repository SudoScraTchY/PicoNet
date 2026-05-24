namespace PicoNet.Application.Features.Shortener.Commands;

public record CreateShortUrlCommand(
    string OriginalUrl,
    string? CustomAlias = null,
    List<string>? Tags = null);