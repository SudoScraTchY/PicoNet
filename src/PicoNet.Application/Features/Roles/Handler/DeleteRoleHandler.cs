using ErrorOr;
using Microsoft.AspNetCore.Identity;
using PicoNet.Application.Features.Roles.Commands;

namespace PicoNet.Application.Features.Roles.Handler;

public sealed class DeleteRoleHandler
{
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public DeleteRoleHandler(RoleManager<IdentityRole<Guid>> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<ErrorOr<Success>> Handle(DeleteRoleCommand command, CancellationToken ct)
    {
        var role = await _roleManager.FindByNameAsync(command.RoleName);
        if (role is null)
            return Error.NotFound("Role.NotFound", $"Role '{command.RoleName}' not found.");

        // Prevent deletion of built-in roles
        if (command.RoleName == "User" || command.RoleName == "Admin")
            return Error.Conflict("Role.CannotDelete", "Built-in roles cannot be deleted.");

        var deleteResult = await _roleManager.DeleteAsync(role);
        if (!deleteResult.Succeeded)
            return Error.Conflict("Role.DeleteFailed", "Failed to delete role.");

        return Result.Success;
    }
}


