using ErrorOr;
using Microsoft.AspNetCore.Identity;
using PicoNet.Application.Features.Auth.Commands;
using PicoNet.Contracts.DTOs.Responses.Auth;
using PicoNet.Infrastructure.Identity;
using PicoNet.Infrastructure.Identity.Entities;

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
        
            var updateResultChangeEmail = await _userManager.UpdateAsync(user);
            if (!updateResultChangeEmail.Succeeded)
                return Error.Failure("User.UpdateFailed", string.Join("; ", updateResultChangeEmail.Errors.Select(e => e.Description)));
            
            Console.WriteLine(tokenForMistype);
            //Send Email
            return new ChangeEmailResponse(user.Email,"Email Sent");
        }

        var existing = await _userManager.FindByEmailAsync(command.NewEmail);
        if (existing is not null)
            return Error.Conflict("User.AlreadyExists", "Email already in use.");

        user.EmailConfirmed = false;
        user.Email = command.NewEmail;
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            return Error.Failure("User.UpdateFailed", string.Join("; ", updateResult.Errors.Select(e => e.Description)));

        // send/log token, same as registration
        Console.WriteLine(token);
        
        return new ChangeEmailResponse(user.Email,"Email Sent");
    }
}