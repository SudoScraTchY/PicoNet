namespace PicoNet.Application.Features.Users.Commands;

public record UpdateUserCommand(
    Guid UserId,
    string? Username = null,
    string? Email = null,
    bool? IsActive = null);

