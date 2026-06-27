namespace PicoNet.Application.Features.Users.Commands;

public record AddRoleCommand(Guid UserId, string RoleName);

