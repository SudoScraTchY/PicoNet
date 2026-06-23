using System.IdentityModel.Tokens.Jwt;
using ErrorOr;
using PicoNet.Application.Features.Auth.Commands;
using PicoNet.Contracts.DTOs.Requests.Auth;

namespace PicoNet.Api.Extensions;

public static class AuthHelper
{
    public static ErrorOr<UserContext> GetCurrentUser(this HttpContext httpContext)
    {
        var userIdString = httpContext?.User?.Claims?.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub);
        if (userIdString is { Value: not null } && Guid.TryParse(userIdString.Value, out var userId))
            return new UserContext(userId);
        
        return Error.Unauthorized();
    }

    public static string? GetAccessToken(this HttpContext httpContext)
    {
        return httpContext.Request.Headers.Authorization.Count > 0
            ? httpContext.Request.Headers.Authorization.ToString()
            : httpContext.Request.Cookies["Authorization"];
    }
    
    public static string? GetRefreshToken(this HttpContext httpContext)
    {
        return httpContext.Request.Headers["RefreshToken"].Count > 0
            ? httpContext.Request.Headers.Authorization.ToString()
            : httpContext.Request.Cookies["RefreshToken"];
    }

    public static ErrorOr<RefreshCommand> GetRefreshCommand(this HttpContext httpContext)
    {
        var refreshToken = httpContext.GetRefreshToken();

        if (refreshToken == null)
            return Error.Unauthorized();

        var userAgentData = httpContext.GetUserAgentData();
        
        UserContext? userContext = null;

        var userIdString =  httpContext.User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub);
        if (userIdString is { Value: not null } && Guid.TryParse(userIdString.Value, out var userId))
            userContext = new UserContext(userId);

        return new RefreshCommand(userAgentData, userContext, refreshToken);
    }

    public static UserAgentData GetUserAgentData(this HttpContext httpContext)
        => new UserAgentData(httpContext.Request.Headers?.UserAgent.ToString(),
            httpContext.Connection.RemoteIpAddress?.ToString(),
            httpContext.Request.Headers?.Referer.ToString());
}