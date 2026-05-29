using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using PicoNet.Application.Features.Shortener.Commands;
using PicoNet.Contracts.DTOs.Requests;
using PicoNet.Contracts.DTOs.Responses;
using PicoNet.Contracts.DTOs.Responses.Shortener;
using Wolverine;

namespace PicoNet.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShortenerController : ControllerBase
{
    private readonly IMessageBus _bus;

    public ShortenerController(IMessageBus bus) => _bus = bus;

    [HttpPost]
    public async Task<IResult> Create([FromBody] CreateShortUrlCommand command)
    {
        var result = await _bus.InvokeAsync<ErrorOr<ShortUrlResponse>>(command);
        return result.Match(
            response => Results.Created($"/api/shortener/{response.ShortCode}", (object?)response),
            Results.BadRequest
        );
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CursorPaginatedResult<ShortUrlResponse>))]
    public async Task<IResult> GetUserShortenedUrls(string? cursor ,int pageSize,CancellationToken ct)
    {
        var result =
            await _bus.InvokeAsync<ErrorOr<CursorPaginatedResult<ShortUrlResponse>>>(
                new CursorPaginatedRequestDto(pageSize,cursor), ct);

        return result.Match(
            Results.Ok,
            Results.BadRequest);
    }
    
    // [HttpGet("{urlId:guid}")]
    // [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CursorPaginatedResult<ShortUrlResponse>))]
    // public async Task<IResult> GetUserShortenedUrls(Guid urlId,int pageSize,CancellationToken ct)
    // {
    //     var result =
    //         await _bus.InvokeAsync<ErrorOr<CursorPaginatedResult<ShortUrlResponse>>>(
    //             new CursorPaginatedRequestDto(pageSize,cursor), ct);
    //
    //     return result.Match(
    //         Results.Ok,
    //         Results.BadRequest);
    // }
}