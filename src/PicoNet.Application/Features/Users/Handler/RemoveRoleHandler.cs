using ErrorOr;
using Microsoft.AspNetCore.Identity;
using PicoNet.Application.Features.Users.Commands;
using PicoNet.Contracts.DTOs.Responses.Users;
using PicoNet.Infrastructure.Identity.Entities;

namespace PicoNet.Application.Features.Users.Handler;

public sealed class RemoveRoleHandler
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public RemoveRoleHandler(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<ErrorOr<UserRolesResponse>> Handle(RemoveRoleCommand command, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(command.UserId.ToString());
        if (user is null)
            return Error.NotFound("User.NotFound", $"User with ID '{command.UserId}' not found.");

        // Check if role exists
        var roleExists = await _roleManager.RoleExistsAsync(command.RoleName);
        if (!roleExists)
            return Error.NotFound("Role.NotFound", $"Role '{command.RoleName}' does not exist.");

        // Check if user has this role
        var userHasRole = await _userManager.IsInRoleAsync(user, command.RoleName);
        if (!userHasRole)
            return Error.NotFound("User.RoleNotFound", $"User does not have role '{command.RoleName}'.");

        var removeResult = await _userManager.RemoveFromRoleAsync(user, command.RoleName);
        if (!removeResult.Succeeded)
            return Error.Conflict("User.RemoveRoleFailed", $"Failed to remove role '{command.RoleName}' from user.");

        var roles = await _userManager.GetRolesAsync(user);

        return new UserRolesResponse
        {
            UserId = user.Id,
            Username = user.UserName ?? string.Empty,
            Roles = roles.ToList()
        };
    }
}

