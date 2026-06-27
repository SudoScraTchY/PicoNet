using ErrorOr;
using Microsoft.AspNetCore.Identity;
using PicoNet.Application.Features.Roles.Commands;
using PicoNet.Contracts.DTOs.Responses.Roles;

namespace PicoNet.Application.Features.Roles.Handler;

public sealed class CreateRoleHandler
{
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public CreateRoleHandler(RoleManager<IdentityRole<Guid>> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<ErrorOr<RoleResponse>> Handle(CreateRoleCommand command, CancellationToken ct)
    {
        // Check if role already exists
        var existingRole = await _roleManager.FindByNameAsync(command.RoleName);
        if (existingRole is not null)
            return Error.Conflict("Role.AlreadyExists", $"Role '{command.RoleName}' already exists.");

        var role = new IdentityRole<Guid>
        {
            Id = Guid.NewGuid(),
            Name = command.RoleName,
            NormalizedName = _roleManager.NormalizeKey(command.RoleName)
        };

        var createResult = await _roleManager.CreateAsync(role);
        if (!createResult.Succeeded)
        {
            var error = createResult.Errors.FirstOrDefault();
            return Error.Conflict("Role.CreationFailed", error?.Description ?? "Failed to create role.");
        }

        return new RoleResponse
        {
            Id = role.Id,
            RoleName = role.Name ?? string.Empty,
            UserCount = 0
        };
    }
}

