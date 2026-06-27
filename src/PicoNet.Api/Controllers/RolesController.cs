using ErrorOr;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PicoNet.Api.Extensions;
using PicoNet.Application.Features.Roles.Commands;
using PicoNet.Contracts.DTOs.Requests.Roles;
using PicoNet.Contracts.DTOs.Responses.Roles;
using Wolverine;

namespace PicoNet.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "RequireAdmin")]
public class RolesController : ControllerBase
{
    private readonly IMessageBus _bus;

    public RolesController(IMessageBus bus) => _bus = bus;

    /// <summary>
    /// List all roles with user counts.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<RoleResponse>))]
    public async Task<IResult> ListRoles(CancellationToken ct = default)
    {
        var result = await _bus.InvokeAsync<ErrorOr<List<RoleResponse>>>(
            new ListRolesCommand(), ct);

        return result.Match(Results.Ok, errors => errors.ToProblemResult());
    }

    /// <summary>
    /// Create a new role.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(RoleResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IResult> CreateRole([FromBody] CreateRoleRequest request, CancellationToken ct = default)
    {
        var result = await _bus.InvokeAsync<ErrorOr<RoleResponse>>(
            new CreateRoleCommand(request.RoleName, request.Description), ct);

        return result.Match(
            role => Results.Created($"/api/roles/{role.RoleName}", role),
            errors => errors.ToProblemResult()
        );
    }

    /// <summary>
    /// Delete a role (built-in roles cannot be deleted).
    /// </summary>
    [HttpDelete("{roleName}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IResult> DeleteRole([FromRoute] string roleName, CancellationToken ct = default)
    {
        var result = await _bus.InvokeAsync<ErrorOr<Success>>(
            new DeleteRoleCommand(roleName), ct);

        return result.Match(
            _ => Results.NoContent(),
            errors => errors.ToProblemResult()
        );
    }
}

