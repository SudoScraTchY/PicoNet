using PicoNet.Contracts.DTOs.Requests;
using PicoNet.Domain.Enums;

namespace PicoNet.Application.Features.Shortener.Commands;

public record EditShortUrlCommand(
    Guid ShortenedUrlId,
    UserContext UserContext,
    string? OriginalUrl,
    string? CustomAlias,
    UrlStatus? UrlStatus,
    List<string>? Tags,
    bool? IsPermanent,
    DateTime? ExpiryTime,
    string? Password,
    string?Campaign);