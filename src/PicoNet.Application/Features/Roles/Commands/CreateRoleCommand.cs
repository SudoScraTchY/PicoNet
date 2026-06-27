namespace PicoNet.Application.Features.Roles.Commands;

public record CreateRoleCommand(string RoleName, string? Description = null);

