using ErrorOr;
using Microsoft.AspNetCore.Identity;
using PicoNet.Application.Features.Roles.Commands;
using PicoNet.Contracts.DTOs.Responses.Roles;
using PicoNet.Infrastructure.Identity.Entities;

namespace PicoNet.Application.Features.Roles.Handler;

public sealed class ListRolesHandler
{
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public ListRolesHandler(RoleManager<IdentityRole<Guid>> roleManager, UserManager<ApplicationUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task<ErrorOr<List<RoleResponse>>> Handle(ListRolesCommand command, CancellationToken ct)
    {
        var roles = _roleManager.Roles.ToList();
        var roleResponses = new List<RoleResponse>();

        foreach (var role in roles)
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name ?? string.Empty);
            roleResponses.Add(new RoleResponse
            {
                Id = role.Id,
                RoleName = role.Name ?? string.Empty,
                UserCount = usersInRole.Count
            });
        }

        return roleResponses;
    }
}


