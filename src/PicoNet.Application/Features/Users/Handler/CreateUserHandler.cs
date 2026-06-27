using ErrorOr;
using Microsoft.AspNetCore.Identity;
using PicoNet.Application.Features.Users.Commands;
using PicoNet.Contracts.DTOs.Responses.Users;
using PicoNet.Infrastructure.Identity.Entities;

namespace PicoNet.Application.Features.Users.Handler;

public sealed class CreateUserHandler
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public CreateUserHandler(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<ErrorOr<UserResponse>> Handle(CreateUserCommand command, CancellationToken ct)
    {
        // Check if user already exists
        var existingUser = await _userManager.FindByEmailAsync(command.Email);
        if (existingUser is not null)
            return Error.Conflict("User.EmailExists", "A user with this email already exists.");

        var existingUserName = await _userManager.FindByNameAsync(command.Username);
        if (existingUserName is not null)
            return Error.Conflict("User.UsernameExists", "A user with this username already exists.");

        // Create new user
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = command.Username,
            Email = command.Email,
            EmailConfirmed = false,
            CreatedAt = DateTime.UtcNow
        };

        var createResult = await _userManager.CreateAsync(user, command.Password);
        if (!createResult.Succeeded)
        {
            var errors = createResult.Errors.Select(e => Error.Conflict("User.CreationFailed", e.Description)).ToList();
            return errors.First();
        }

        // Add roles if provided
        if (command.Roles is { Count: > 0 })
        {
            foreach (var roleName in command.Roles)
            {
                var roleExists = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                    return Error.Validation("User.InvalidRole", $"Role '{roleName}' does not exist.");

                var addRoleResult = await _userManager.AddToRoleAsync(user, roleName);
                if (!addRoleResult.Succeeded)
                    return Error.Conflict("User.AddRoleFailed", $"Failed to add role '{roleName}' to user.");
            }
        }

        // Add default "User" role if no roles provided
        if (command.Roles is not { Count: > 0 })
        {
            var userRoleExists = await _roleManager.RoleExistsAsync("User");
            if (userRoleExists)
                await _userManager.AddToRoleAsync(user, "User");
        }

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

