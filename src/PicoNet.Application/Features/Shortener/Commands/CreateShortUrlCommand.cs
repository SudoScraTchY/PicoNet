namespace PicoNet.Application.Features.Shortener.Commands;

public record CreateShortUrlCommand(
    string OriginalUrl,
    DateTime? ExpiryTime = null,
    string? Campaign = null,
    int? MaxClicks = null,
    string? CustomAlias = null,
    List<string>? Tags = null,
    string? Password = null);