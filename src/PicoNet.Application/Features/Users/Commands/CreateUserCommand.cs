namespace PicoNet.Application.Features.Users.Commands;

public record CreateUserCommand(
    string Username,
    string Email,
    string Password,
    List<string>? Roles = null);

