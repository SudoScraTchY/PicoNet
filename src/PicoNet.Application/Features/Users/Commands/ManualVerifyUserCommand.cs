namespace PicoNet.Application.Features.Users.Commands;

public record ManualVerifyUserCommand(
    Guid UserId,
    bool SetEmailConfirmed = true,
    bool SetPhoneConfirmed = false);

