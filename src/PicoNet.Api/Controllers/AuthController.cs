using Microsoft.AspNetCore.Mvc;
using PicoNet.Application.Features.Auth.Commands;
using PicoNet.Contracts.DTOs.Requests.Auth;
using Wolverine;
using ErrorOr;
using PicoNet.Api.Extensions;

namespace PicoNet.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMessageBus _bus;
    public AuthController(IMessageBus bus) => _bus = bus;

    [HttpPost("register")]
    public async Task<IResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<ErrorOr<AuthResponse>>(
            new RegisterCommand(request.Email, request.Password), ct);

        return result.Match(Results.Ok, errors => errors.ToProblemResult());
    }

    [HttpPost("login")]
    public async Task<IResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<ErrorOr<AuthResponse>>(
            new LoginCommand(request.Email, request.Password), ct);

        return result.Match(Results.Ok, errors => errors.ToProblemResult());
    }
}