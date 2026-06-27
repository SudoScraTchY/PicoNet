using ErrorOr;
using Microsoft.AspNetCore.Identity;
using PicoNet.Application.Features.Users.Commands;
using PicoNet.Infrastructure.Identity.Entities;

namespace PicoNet.Application.Features.Users.Handler;

public sealed class DeleteUserHandler
{
    private readonly UserManager<ApplicationUser> _userManager;

    public DeleteUserHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ErrorOr<Success>> Handle(DeleteUserCommand command, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(command.UserId.ToString());
        if (user is null)
            return Error.NotFound("User.NotFound", $"User with ID '{command.UserId}' not found.");

        var deleteResult = await _userManager.DeleteAsync(user);
        if (!deleteResult.Succeeded)
            return Error.Conflict("User.DeleteFailed", "Failed to delete user.");

        return Result.Success;
    }
}

