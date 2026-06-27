using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ErrorOr;
using ImTools;
using PicoNet.Application.Features.Auth.Commands;
using PicoNet.Contracts.DTOs.Requests.Auth;

namespace PicoNet.Api.Extensions;

public static class AuthHelper
{
    public static ErrorOr<UserContext> GetCurrentUser(this HttpContext httpContext)
    {
        var userIdString = httpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)
                           ?? httpContext?.User?.FindFirst(JwtRegisteredClaimNames.Sub);

        var list = httpContext?.User?.List();

        if (userIdString is { Value: not null } && Guid.TryParse(userIdString.Value, out var userId))
        {
            var roles = httpContext?.User?.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();
            
            return new UserContext(userId,roles ?? []);
        }

        return Error.Unauthorized();
    }

    public static string? GetAccessToken(this HttpContext httpContext)
    {
        return httpContext.Request.Headers.Authorization.Count > 0
            ? httpContext.Request.Headers.Authorization.ToString()
            : httpContext.Request.Cookies["Authorization"];
    }

    public static void SetAccessToken(this HttpResponse response, string accessToken,DateTime expires)
    {
        response.Cookies.Append("Authorization", accessToken,new CookieOptions()
        {
            Expires = expires,
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
        });
        response.Headers.Authorization = accessToken;
    }
    
    public static void SetRefreshToken(this HttpResponse response, string refreshToken,DateTime expires)
    {
        response.Cookies.Append("RefreshToken", refreshToken,new CookieOptions()
        {
            Expires = expires,
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
        });
        response.Headers["refreshToken"] = refreshToken;
    }
    
    public static string? GetRefreshToken(this HttpContext httpContext)
    {
        return httpContext.Request.Headers["RefreshToken"].Count > 0
            ? httpContext.Request.Headers["RefreshToken"].ToString()
            : httpContext.Request.Cookies["RefreshToken"];
    }

    public static UserAgentData GetUserAgentData(this HttpContext httpContext)
        => new UserAgentData(httpContext.Request.Headers?.UserAgent.ToString(),
            httpContext.Connection.RemoteIpAddress?.ToString(),
            httpContext.Request.Headers?.Referer.ToString());
}