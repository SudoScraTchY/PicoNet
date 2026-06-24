using ErrorOr;
using Microsoft.AspNetCore.Identity;
using PicoNet.Application.Features.Auth.Commands;
using PicoNet.Application.Mappings;
using PicoNet.Contracts.DTOs.Responses.Auth;
using PicoNet.Infrastructure.Identity;
using PicoNet.Infrastructure.Identity.Entities;
using PicoNet.Infrastructure.Identity.Interfaces;

namespace PicoNet.Application.Features.Auth.Handler;

public sealed class LoginHandler
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;

    public LoginHandler(UserManager<ApplicationUser> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<ErrorOr<AuthResponse>> Handle(LoginCommand command, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(command.Email) && string.IsNullOrEmpty(command.Username))
        {
            return Error.Validation("Auth.InvalidCredentials", "No Email or Username was provided.");
        }
        
        var user = !string.IsNullOrEmpty(command.Email) ?
            await _userManager.FindByEmailAsync(command.Email) : !string.IsNullOrEmpty(command.Username) ?
             await _userManager.FindByNameAsync(command.Username) : null;
        
        if (user is null)
            return Error.Unauthorized("Auth.InvalidCredentials", "Invalid email or password.");

        var passwordValid = await _userManager.CheckPasswordAsync(user, command.Password);
        if (!passwordValid)
            return Error.Unauthorized("Auth.InvalidCredentials", "Invalid email or password.");

        var (token, expiresAt) = _tokenService.GenerateToken(user);
        
        var (refreshToken, refreshExpiresAt) =
            await _tokenService.GenerateRefreshTokenAsync(user, command.UserAgentData.IpAddress,
                command.UserAgentData.UserAgent, null, ct);
        
        return new AuthResponse(new AuthTokenResponse(token,expiresAt, refreshToken, refreshExpiresAt),
            user.ToAuthResponseUser());
    }
}