namespace PicoNet.Application.Features.Users.Commands;

public record ListUsersCommand(int PageNumber = 1, int PageSize = 10);

