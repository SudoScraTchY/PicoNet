using ErrorOr;
using Microsoft.AspNetCore.Identity;
using PicoNet.Application.Features.Users.Commands;
using PicoNet.Contracts.DTOs.Responses.Users;
using PicoNet.Infrastructure.Identity.Entities;

namespace PicoNet.Application.Features.Users.Handler;

public sealed class UpdateUserHandler
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UpdateUserHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ErrorOr<UserResponse>> Handle(UpdateUserCommand command, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(command.UserId.ToString());
        if (user is null)
            return Error.NotFound("User.NotFound", $"User with ID '{command.UserId}' not found.");

        // Update username if provided
        if (!string.IsNullOrWhiteSpace(command.Username) && command.Username != user.UserName)
        {
            var existingUser = await _userManager.FindByNameAsync(command.Username);
            if (existingUser is not null && existingUser.Id != user.Id)
                return Error.Conflict("User.UsernameExists", "A user with this username already exists.");

            user.UserName = command.Username;
        }

        // Update email if provided
        if (!string.IsNullOrWhiteSpace(command.Email) && command.Email != user.Email)
        {
            var existingUser = await _userManager.FindByEmailAsync(command.Email);
            if (existingUser is not null && existingUser.Id != user.Id)
                return Error.Conflict("User.EmailExists", "A user with this email already exists.");

            user.Email = command.Email;
            user.NormalizedEmail = _userManager.NormalizeEmail(command.Email);
        }

        // Update lock status if provided (IsActive controls lockout)
        if (command.IsActive.HasValue)
        {
            if (command.IsActive.Value)
            {
                // Reset lockout
                user.LockoutEnd = null;
                user.AccessFailedCount = 0;
            }
            else
            {
                // Lock user indefinitely
                user.LockoutEnd = DateTimeOffset.MaxValue;
            }
        }

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            return Error.Conflict("User.UpdateFailed", "Failed to update user.");

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

