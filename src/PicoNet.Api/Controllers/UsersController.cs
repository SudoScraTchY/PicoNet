using ErrorOr;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PicoNet.Api.Extensions;
using PicoNet.Application.Features.Users.Commands;
using PicoNet.Contracts.DTOs.Requests.Users;
using PicoNet.Contracts.DTOs.Responses.Users;
using Wolverine;

namespace PicoNet.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "RequireAdmin")]
public class UsersController : ControllerBase
{
    private readonly IMessageBus _bus;

    public UsersController(IMessageBus bus) => _bus = bus;

    /// <summary>
    /// List all users with pagination support.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ListUsersResponse))]
    public async Task<IResult> ListUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
    {
        var result = await _bus.InvokeAsync<ErrorOr<ListUsersResponse>>(
            new ListUsersCommand(pageNumber, pageSize), ct);

        return result.Match(Results.Ok, errors => errors.ToProblemResult());
    }

    /// <summary>
    /// Get a specific user by ID.
    /// </summary>
    [HttpGet("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetUser([FromRoute] Guid userId, CancellationToken ct = default)
    {
        var result = await _bus.InvokeAsync<ErrorOr<UserResponse>>(
            new GetUserCommand(userId), ct);

        return result.Match(Results.Ok, errors => errors.ToProblemResult());
    }

    /// <summary>
    /// Create a new user with optional roles.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UserResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IResult> CreateUser([FromBody] CreateUserRequest request, CancellationToken ct = default)
    {
        var result = await _bus.InvokeAsync<ErrorOr<UserResponse>>(
            new CreateUserCommand(request.Username, request.Email, request.Password, request.Roles), ct);

        return result.Match(
            user => Results.Created($"/api/users/{user.Id}", user),
            errors => errors.ToProblemResult()
        );
    }

    /// <summary>
    /// Update user details (username, email, status).
    /// </summary>
    [HttpPatch("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> UpdateUser([FromRoute] Guid userId, [FromBody] UpdateUserRequest request, CancellationToken ct = default)
    {
        var result = await _bus.InvokeAsync<ErrorOr<UserResponse>>(
            new UpdateUserCommand(userId, request.Username, request.Email, request.IsActive), ct);

        return result.Match(Results.Ok, errors => errors.ToProblemResult());
    }

    /// <summary>
    /// Delete a user permanently.
    /// </summary>
    [HttpDelete("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> DeleteUser([FromRoute] Guid userId, CancellationToken ct = default)
    {
        var result = await _bus.InvokeAsync<ErrorOr<Success>>(
            new DeleteUserCommand(userId), ct);

        return result.Match(
            _ => Results.NoContent(),
            errors => errors.ToProblemResult()
        );
    }

    /// <summary>
    /// Manually verify a user (confirm email/phone).
    /// </summary>
    [HttpPost("{userId:guid}/verify")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> ManualVerifyUser([FromRoute] Guid userId, [FromBody] ManualVerifyUserRequest request, CancellationToken ct = default)
    {
        var result = await _bus.InvokeAsync<ErrorOr<UserResponse>>(
            new ManualVerifyUserCommand(userId, request.SetEmailConfirmed, request.SetPhoneConfirmed), ct);

        return result.Match(Results.Ok, errors => errors.ToProblemResult());
    }

    /// <summary>
    /// Force change a user's password (admin action).
    /// </summary>
    [HttpPost("{userId:guid}/force-password-change")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IResult> ForcePasswordChange([FromRoute] Guid userId, [FromBody] ForcePasswordChangeRequest request, CancellationToken ct = default)
    {
        var result = await _bus.InvokeAsync<ErrorOr<UserResponse>>(
            new ForcePasswordChangeCommand(userId, request.NewPassword, request.SendEmailNotification ?? true), ct);

        return result.Match(Results.Ok, errors => errors.ToProblemResult());
    }

    /// <summary>
    /// Add a role to a user.
    /// </summary>
    [HttpPost("{userId:guid}/roles")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserRolesResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IResult> AddRole([FromRoute] Guid userId, [FromBody] ManageRoleRequest request, CancellationToken ct = default)
    {
        var result = await _bus.InvokeAsync<ErrorOr<UserRolesResponse>>(
            new AddRoleCommand(userId, request.RoleName), ct);

        return result.Match(Results.Ok, errors => errors.ToProblemResult());
    }

    /// <summary>
    /// Remove a role from a user.
    /// </summary>
    [HttpDelete("{userId:guid}/roles/{roleName}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserRolesResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> RemoveRole([FromRoute] Guid userId, [FromRoute] string roleName, CancellationToken ct = default)
    {
        var result = await _bus.InvokeAsync<ErrorOr<UserRolesResponse>>(
            new RemoveRoleCommand(userId, roleName), ct);

        return result.Match(Results.Ok, errors => errors.ToProblemResult());
    }
}

