using PicoNet.Contracts.DTOs.Requests;
using PicoNet.Contracts.DTOs.Requests.Auth;

namespace PicoNet.Application.Features.Shortener.Commands;

public record RemoveShortUrlCommand(Guid UrlId,UserContext UserContext);