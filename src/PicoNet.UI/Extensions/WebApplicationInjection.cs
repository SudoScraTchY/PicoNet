using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using PicoNet.Contracts.DTOs.Requests.Auth;
using PicoNet.UI.ApiClients.Interfaces;

namespace PicoNet.UI.Extensions;

public static class WebApplicationInjection
{
    // public static void AddPicoNetAuthInternalHandler(this WebApplication app)
    // {
    //     app.MapPost("/api/auth/login", async (
    //         LoginRequest request, 
    //         IAuthApiClient authClient, 
    //         HttpContext httpContext) =>
    //     {
    //         var result = await authClient.LoginAsync(request, CancellationToken.None);
    //         if (result.IsError) return result.Errors.ToProblemResult();
    //
    //         // Set cookies on the BROWSER's response
    //         httpContext.Response.Cookies.Append("Access_Token", result.Value.Tokens.AccessToken, new()
    //         {
    //             HttpOnly = true, Secure = true, SameSite = SameSiteMode.Strict,
    //             Expires = result.Value.Tokens.AccessExpiresAt
    //         });
    //
    //         httpContext.Response.Cookies.Append("Refresh_Token", result.Value.Tokens.RefreshToken, new()
    //         {
    //             HttpOnly = true, Secure = true, SameSite = SameSiteMode.Strict,
    //             Expires = result.Value.Tokens.RefreshExpiresAt
    //         });
    //
    //         // Sign in to ASP.NET Core auth
    //         var claims = new[] 
    //         { 
    //             new Claim(ClaimTypes.NameIdentifier, result.Value.User.Id.ToString()),
    //             new Claim(ClaimTypes.Email, result.Value.User.Email) 
    //         };
    //
    //         await httpContext.SignInAsync(
    //             CookieAuthenticationDefaults.AuthenticationScheme,
    //             new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)));
    //
    //         // Redirect to dashboard — browser follows this, cookies are already set
    //         return Results.Redirect("/dashboard");
    //     });
    // }
}