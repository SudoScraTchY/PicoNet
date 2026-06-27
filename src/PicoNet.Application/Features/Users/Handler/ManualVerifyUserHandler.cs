using ErrorOr;
using Microsoft.AspNetCore.Identity;
using PicoNet.Application.Features.Users.Commands;
using PicoNet.Contracts.DTOs.Responses.Users;
using PicoNet.Infrastructure.Identity.Entities;

namespace PicoNet.Application.Features.Users.Handler;

public sealed class ManualVerifyUserHandler
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ManualVerifyUserHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ErrorOr<UserResponse>> Handle(ManualVerifyUserCommand command, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(command.UserId.ToString());
        if (user is null)
            return Error.NotFound("User.NotFound", $"User with ID '{command.UserId}' not found.");

        // Set email confirmed
        if (command.SetEmailConfirmed)
        {
            user.EmailConfirmed = true;
        }

        // Set phone confirmed
        if (command.SetPhoneConfirmed)
        {
            user.PhoneNumberConfirmed = true;
        }

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            return Error.Conflict("User.VerificationFailed", "Failed to verify user.");

        var roles = await _userManager.GetRolesAsync(user);

        return new UserResponse
        {
            Id = user.Id,
            Username = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            EmailConfirmed = user.EmailConfirmed,
            TwoFactorEnabled = user.TwoFactorEnabled,
            LockoutEnabled = user.LockoutEnabled,
            AccessFailedCount = user.AccessFailedCount,
            CreatedAt = user.CreatedAt,
            Roles = roles.ToList()
        };
    }
}

