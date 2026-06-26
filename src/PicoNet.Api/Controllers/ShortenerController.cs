using ErrorOr;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PicoNet.Api.Extensions;
using PicoNet.Application.Features.Shortener.Commands;
using PicoNet.Application.Features.Shortener.Queries;
using PicoNet.Contracts.DTOs.Requests.Shortener;
using PicoNet.Contracts.DTOs.Responses;
using PicoNet.Contracts.DTOs.Responses.Shortener;
using Wolverine;

namespace PicoNet.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ShortenerController : ControllerBase
{
    private readonly IMessageBus _bus;

    public ShortenerController(IMessageBus bus)
    {
        _bus = bus;
    }

    [HttpPost]
    public async Task<IResult> CreateUserShortUrl([FromBody] CreateShortUrlRequest request,CancellationToken ct)
    {
        var userCtx = HttpContext.GetCurrentUser();
        if(userCtx.IsError)
            return  Results.Unauthorized();
        
        var command = new CreateShortUrlCommand(
            OriginalUrl: request.OriginalUrl,
            userCtx.Value,
            CustomAlias: request.CustomAlias,
            Tags: request.Tags,
            MaxClicks: request.MaxClicks,
            Password: request.Password,
            ExpiryTime: request.ExpiryTime,
            Campaign: request.Campaign);
        
        var result = await _bus.InvokeAsync<ErrorOr<ShortUrlResponse>>(command, ct);
        
        return result.Match(
            response => Results.Created($"/api/shortener/{response.ShortCode}", (object?)response),
            Results.BadRequest
        );
    }
    
    [HttpPut("{urlId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ShortUrlResponse))]
    public async Task<IResult> EditUserShortUrl(Guid urlId,[FromBody] EditShortUrlRequest editShortUrlRequest,CancellationToken ct)
    {
        var userCtx = HttpContext.GetCurrentUser();
        if(userCtx.IsError)
            return  Results.Unauthorized();
                
        var result = await _bus.InvokeAsync<ErrorOr<ShortUrlResponse>>(
                new EditShortUrlCommand(urlId,userCtx.Value,editShortUrlRequest.OriginalUrl,editShortUrlRequest.CustomAlias,
                    editShortUrlRequest.UrlStatus,editShortUrlRequest.Tags,editShortUrlRequest.IsPermanent,
                    editShortUrlRequest.ExpiryTime, editShortUrlRequest.Password,editShortUrlRequest.Campaign),
                ct);

        return result.Match(
            Results.Ok,
            Results.BadRequest);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CursorPaginatedResult<ShortUrlResponse>))]
    public async Task<IResult> GetUserShortenedUrls(string? cursor ,int pageSize,CancellationToken ct)
    {
        var test = Request;
        var userCtx = HttpContext.GetCurrentUser();
        if(userCtx.IsError)
            return  Results.Unauthorized();
        
        var result = await _bus.InvokeAsync<ErrorOr<CursorPaginatedResult<ShortUrlResponse>>>(
                new CursorPaginatedCommand(userCtx.Value,pageSize,cursor), ct);

        return result.Match(
            Results.Ok,
            Results.BadRequest);
    }
    
    [HttpGet("{urlId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ShortUrlResponse))]
    public async Task<IResult> GetUserShortenedUrl(Guid urlId,CancellationToken ct)
    {
        var userCtx = HttpContext.GetCurrentUser();
        if(userCtx.IsError)
            return  Results.Unauthorized();
        
        var result = await _bus.InvokeAsync<ErrorOr<ShortUrlResponse>>(new GetShortUrlByIdQuery(userCtx.Value,urlId), ct);
    
        return result.Match(
            Results.Ok,
            Results.BadRequest);
    }
}