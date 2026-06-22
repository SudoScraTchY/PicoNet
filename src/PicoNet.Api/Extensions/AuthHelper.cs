using System.IdentityModel.Tokens.Jwt;
using ErrorOr;
using PicoNet.Contracts.DTOs.Requests;
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

    public static UserAgentData GetUserAgentData(this HttpContext httpContext)
        => new UserAgentData(httpContext.Request.Headers?.UserAgent.ToString(),
            httpContext.Connection.RemoteIpAddress?.ToString(),
            httpContext.Request.Headers?.Referer.ToString());
}