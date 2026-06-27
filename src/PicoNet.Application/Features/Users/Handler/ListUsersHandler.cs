using ErrorOr;
using Microsoft.AspNetCore.Identity;
using PicoNet.Application.Features.Users.Commands;
using PicoNet.Contracts.DTOs.Responses.Users;
using PicoNet.Infrastructure.Identity.Entities;

namespace PicoNet.Application.Features.Users.Handler;

public sealed class ListUsersHandler
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ListUsersHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ErrorOr<ListUsersResponse>> Handle(ListUsersCommand command, CancellationToken ct)
    {
        var totalCount = _userManager.Users.Count();
        var pageNumber = command.PageNumber < 1 ? 1 : command.PageNumber;
        var pageSize = command.PageSize < 1 ? 10 : command.PageSize;
        var skip = (pageNumber - 1) * pageSize;

        var users = _userManager.Users
            .OrderBy(u => u.UserName)
            .Skip(skip)
            .Take(pageSize)
            .ToList();

        var userResponses = new List<UserResponse>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userResponses.Add(new UserResponse
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
            });
        }

        return new ListUsersResponse
        {
            Users = userResponses,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}

