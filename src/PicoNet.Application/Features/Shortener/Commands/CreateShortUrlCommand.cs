namespace PicoNet.Application.Features.Shortener.Commands;

public record CreateShortUrlCommand(
    string OriginalUrl,
    string IpAddress,
    string UserAgent,
    DateTime? ExpiryTime = null,
    string? Campaign = null,
    int? MaxClicks = null,
    string? CustomAlias = null,
    List<string>? Tags = null,
    string? Password = null);