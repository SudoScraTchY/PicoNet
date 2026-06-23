using PicoNet.Contracts.DTOs.Requests;
using PicoNet.Contracts.DTOs.Requests.Auth;

namespace PicoNet.Application.Features.Shortener.Queries;

public record GetShortUrlByIdQuery(UserContext UserContext,Guid UrlId);