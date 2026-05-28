using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using PicoNet.Application.Features.Shortener.Commands;
using PicoNet.Contracts.DTOs.Requests;
using PicoNet.Contracts.DTOs.Responses;
using PicoNet.Contracts.DTOs.Responses.Shortner;
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

    public async Task<ErrorOr<PaginatedResult<ShortUrlResponse>>> GetUserShortenedUrls(int pageNumber,int pageSize,CancellationToken ct)
    {
        var result =
            await _bus.InvokeAsync<ErrorOr<PaginatedResult<ShortUrlResponse>>>(
                new PaginatedRequestDto(pageNumber, pageSize, HttpContext), ct);
        
        return result;
    }
}