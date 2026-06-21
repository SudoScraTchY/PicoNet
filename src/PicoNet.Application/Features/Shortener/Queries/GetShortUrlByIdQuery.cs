using PicoNet.Contracts.DTOs.Requests;

namespace PicoNet.Application.Features.Shortener.Queries;

public record GetShortUrlByIdQuery(UserContext UserContext,Guid UrlId);