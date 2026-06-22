using ErrorOr;
using Microsoft.AspNetCore.Identity;
using PicoNet.Application.Mappings;
using PicoNet.Contracts.DTOs.Requests.Auth;
using PicoNet.Contracts.DTOs.Responses.Auth;
using PicoNet.Infrastructure.Identity;
using PicoNet.Infrastructure.Identity.Interfaces;

namespace PicoNet.Application.Features.Auth.Handler;

public sealed class ValidateRegistrationHandler
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;

    public ValidateRegistrationHandler(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public async Task<ErrorOr<AuthResponse>> Handle(ValidateRegistrationRequest command, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(command.Email);
        if (user is null)
            return Error.Validation("User.Email", $"User with email {command.Email} not found");

        user.EmailConfirmed = true;

        var result = await _userManager.ConfirmEmailAsync(user, command.Token);

        if (!result.Succeeded)
        {
            return Error.Failure("User.Email", result.Errors.First().Description);
        }

        var (token, expiresAt) = _tokenService.GenerateToken(user);
        return new AuthResponse(AccessToken: token, RefreshToken: string.Empty, ExpiresAt: expiresAt,
            User: user.ToAuthResponseUser());
    }
}