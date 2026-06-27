namespace PicoNet.Application.Features.Users.Commands;

public record ForcePasswordChangeCommand(
    Guid UserId,
    string NewPassword,
    bool SendEmailNotification = true);

