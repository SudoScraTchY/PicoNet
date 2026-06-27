namespace PicoNet.Application.Features.Users.Commands;

public record RemoveRoleCommand(Guid UserId, string RoleName);

