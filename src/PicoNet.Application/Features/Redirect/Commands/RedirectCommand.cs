namespace PicoNet.Application.Features.Redirect.Commands;

public record RedirectCommand(
    string ShortCode,
    string? IpAddress,
    string UserAgent,
    string? Referrer = null
    ,string? Password = null);