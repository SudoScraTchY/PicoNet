using Microsoft.AspNetCore.Mvc;
using PicoNet.Application.Features.Auth.Commands;
using PicoNet.Contracts.DTOs.Requests.Auth;
using Wolverine;
using ErrorOr;
using Microsoft.AspNetCore.Authorization;
using PicoNet.Api.Extensions;
using PicoNet.Application.Features.Auth.Handler;
using PicoNet.Contracts.DTOs.Responses.Auth;

namespace PicoNet.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    //private readonly IMessageBus _bus;
    
    private readonly RegisterHandler _registerHandler;
    private readonly LoginHandler _loginHandler;
    private readonly ChangeEmailHandler _changeEmailHandler;
    private readonly ValidateRegistrationHandler _validateRegistrationHandler;
    private readonly RefreshHandler _refreshHandler;
    public AuthController(ValidateRegistrationHandler validateRegistrationHandler, RegisterHandler registerHandler, LoginHandler loginHandler, ChangeEmailHandler changeEmailHandler, RefreshHandler refreshHandler)
    {
        _validateRegistrationHandler = validateRegistrationHandler;
        _registerHandler = registerHandler;
        _loginHandler = loginHandler;
        _changeEmailHandler = changeEmailHandler;
        _refreshHandler = refreshHandler;
    }

    [HttpPost("register")]
    public async Task<IResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var result = await _registerHandler.Handle(
            new RegisterCommand(request.Username, request.Email, request.Password, HttpContext.GetUserAgentData()), ct);
        
        if (result.IsSuccess)
        {
            Response .SetAccessToken(result.Value.Tokens.AccessToken,result.Value.Tokens.AccessExpiresAt);
            Response .SetRefreshToken(result.Value.Tokens.RefreshToken,result.Value.Tokens.RefreshExpiresAt);
        }

        return result.Match(Results.Ok, errors => errors.ToProblemResult());
    }

    [HttpPost("login")]
    public async Task<IResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await _loginHandler.Handle(
            new LoginCommand(request.Email, request.Username, request.Password, HttpContext.GetUserAgentData()), ct);
        
        if (result.IsSuccess)
        {
            Response.SetAccessToken(result.Value.Tokens.AccessToken,result.Value.Tokens.AccessExpiresAt);
            Response.SetRefreshToken(result.Value.Tokens.RefreshToken,result.Value.Tokens.RefreshExpiresAt);
        }

        return result.Match(Results.Ok, errors => errors.ToProblemResult());
    }
    
    [HttpPost("confirm-email")]
    public async Task<IResult> ConfirmEmail([FromBody] ValidateRegistrationRequest request, CancellationToken ct)
    {
        if (!Guid.TryParse(request.Id, out var id))
        {
            return Results.BadRequest();
        } 
                
        var result = await _validateRegistrationHandler.Handle(
            new ValidateEmailCommand(id, request.Token, HttpContext.GetUserAgentData()), ct);

        return result.Match(Results.Ok, errors => errors.ToProblemResult());
    }
    
    [HttpGet("Refresh")]
    public async Task<IResult> RefreshToken(CancellationToken ct)
    {
        var refreshToken = HttpContext.GetRefreshToken();
        if (string.IsNullOrEmpty(refreshToken))
        {
            return Results.Forbid();
        }
        var result = await _refreshHandler.Handle(new RefreshCommand(HttpContext.GetUserAgentData(),refreshToken), ct);

        if (result.IsSuccess)
        {
            Response .SetAccessToken(result.Value.Tokens.AccessToken,result.Value.Tokens.AccessExpiresAt);
            Response .SetRefreshToken(result.Value.Tokens.RefreshToken,result.Value.Tokens.RefreshExpiresAt);
        }
        

        return result.Match(Results.Ok, errors => errors.ToProblemResult());
    }
    
    [HttpPatch("ChangeEmail")]
    [Authorize]
    public async Task<IResult> ChangeEmail(ChangeEmailRequest request,CancellationToken ct)
    {
        var userCtx = HttpContext.GetCurrentUser();
        if(userCtx.IsError)
            return  Results.Unauthorized();

        var result = await _changeEmailHandler.Handle(
            new ChangeEmailCommand(userCtx.Value, request.NewEmail, HttpContext.GetUserAgentData()), ct);

        return result.Match(Results.Ok, errors => errors.ToProblemResult());
    }
}