using PicoNet.Contracts.DTOs.Requests;

namespace PicoNet.Application.Features.Shortener.Commands;

public record RemoveShortUrlCommand(Guid UrlId,UserContext UserContext);