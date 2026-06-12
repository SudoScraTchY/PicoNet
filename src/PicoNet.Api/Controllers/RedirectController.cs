using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using PicoNet.Application.Features.Redirect.Commands;
using PicoNet.Contracts.DTOs.Responses.Redirect;
using Wolverine;

namespace PicoNet.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RedirectController : ControllerBase
{
    private readonly IMessageBus _bus;
    public RedirectController(IMessageBus bus) => _bus = bus;

    [HttpGet("{shortCode}")]
    public async Task<IResult> Redirect([FromRoute] string shortCode, [FromQuery] string? password, CancellationToken ct)

    {
        var command = new RedirectCommand(
            ShortCode: shortCode,
            Password: password,
            IpAddress: HttpContext.Connection.RemoteIpAddress?.ToString(),
            UserAgent: Request.Headers.UserAgent.ToString(),
            Referrer: Request.Headers.Referer.ToString()
        );

        var result = await _bus.InvokeAsync<ErrorOr<RedirectUrlResult>>(command, ct);

        return result.Match(
            redirect => Results.Redirect(redirect.OriginalUrl, permanent: false),
            errors => errors.First().Type switch
            {
                ErrorType.NotFound => Results.NotFound(),
                ErrorType.Conflict => Results.StatusCode(410), // 410 Gone
                ErrorType.Unauthorized => Results.StatusCode(401),
                _ => Results.StatusCode(500)
            }
        );
    }
}