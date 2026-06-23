using ErrorOr;
using Microsoft.AspNetCore.Identity;
using PicoNet.Application.Features.Auth.Commands;
using PicoNet.Contracts.DTOs.Responses.Auth;
using PicoNet.Infrastructure.Identity;

namespace PicoNet.Application.Features.Auth.Handler;

public sealed class ChangeEmailHandler
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ChangeEmailHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }
    
    public async Task<ErrorOr<ChangeEmailResponse>> Handle(ChangeEmailCommand command, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(command.UserContext.UserId.ToString());
        if (user is null) 
            return Error.NotFound();

        // Probably a mistyped Email
        if (user is { EmailConfirmed: false, NormalizedEmail: not null } 
            && user.NormalizedEmail.Equals(command.NewEmail, StringComparison.CurrentCultureIgnoreCase))
        {
            user.Email = command.NewEmail;
            var tokenForMistype = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            //Send Email
        
            await _userManager.UpdateAsync(user);
            return new ChangeEmailResponse(user.Email,"Email Sent");
        }

        var existing = await _userManager.FindByEmailAsync(command.NewEmail);
        if (existing is not null)
            return Error.Conflict("User.AlreadyExists", "Email already in use.");
        
        user.Email = command.NewEmail;
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        await _userManager.UpdateAsync(user);

        // send/log token, same as registration
        

        return new ChangeEmailResponse(user.Email,"Email Sent");
    }
}