using PicoNet.Contracts.DTOs.Requests;
using PicoNet.Contracts.DTOs.Requests.Auth;

namespace PicoNet.Application.Features.Shortener.Commands;

public record CreateShortUrlCommand(
    string OriginalUrl,
    UserContext UserContext,
    DateTime? ExpiryTime = null,
    string? Campaign = null,
    int? MaxClicks = null,
    string? CustomAlias = null,
    List<string>? Tags = null,
    string? Password = null);