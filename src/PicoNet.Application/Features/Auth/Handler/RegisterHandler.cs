using ErrorOr;
using Microsoft.AspNetCore.Identity;
using PicoNet.Application.Features.Auth.Commands;
using PicoNet.Application.Mappings;
using PicoNet.Contracts.DTOs.Requests.Auth;
using PicoNet.Contracts.DTOs.Responses.Auth;
using PicoNet.Infrastructure.Identity;
using PicoNet.Infrastructure.Identity.Entities;
using PicoNet.Infrastructure.Identity.Interfaces;

namespace PicoNet.Application.Features.Auth.Handler;

public sealed class RegisterHandler
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;

    public RegisterHandler(UserManager<ApplicationUser> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<ErrorOr<AuthResponse>> Handle(RegisterCommand command, CancellationToken ct)
    {
        var existing = await _userManager.FindByEmailAsync(command.Email);
        if (existing is not null)
            return Error.Conflict("User.AlreadyExists", "An account with this email already exists.");

        var user = new ApplicationUser { UserName = command.Username, Email = command.Email };
        var result = await _userManager.CreateAsync(user, command.Password);

        if (!result.Succeeded)
        {
            var description = string.Join("; ", result.Errors.Select(e => e.Description));
            return Error.Validation("User.InvalidPassword", description);
        }

        var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        Console.WriteLine($"\n{emailConfirmationToken} has been Created for {command.Email}.\n");
        
        var (token, expiresAt) = _tokenService.GenerateToken(user);
        
        var (refreshToken, refreshExpiresAt) =
            await _tokenService.GenerateRefreshTokenAsync(user, command.UserAgentData.IpAddress,
                command.UserAgentData.UserAgent, null, ct);
        
        return new AuthResponse(new AuthTokenResponse(token,expiresAt, refreshToken, refreshExpiresAt),
            user.ToAuthResponseUser());
    }
}