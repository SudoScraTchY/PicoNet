using ErrorOr;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using PicoNet.Application.Features.Auth.Commands;
using PicoNet.Application.Mappings;
using PicoNet.Contracts.DTOs.Requests.Auth;
using PicoNet.Contracts.DTOs.Responses.Auth;
using PicoNet.Infrastructure.Identity;
using PicoNet.Infrastructure.Identity.Entities;
using PicoNet.Infrastructure.Identity.Interfaces;
using PicoNet.Infrastructure.Services;

namespace PicoNet.Application.Features.Auth.Handler;

public sealed class RegisterHandler
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _config;

    public RegisterHandler(UserManager<ApplicationUser> userManager, ITokenService tokenService, IEmailService emailService, IConfiguration config)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _emailService = emailService;
        _config = config;
    }

    public async Task<ErrorOr<AuthResponse>> Handle(RegisterCommand command, CancellationToken ct)
    {
        var existing = await _userManager.FindByEmailAsync(command.Email);
        if (existing is not null)
            return Error.Conflict("User.AlreadyExists", "An account with this email already exists.");

        var user = new ApplicationUser { UserName = command.Username, Email = command.Email };
        var result = await _userManager.CreateAsync(user, command.Password);

        if (!result.Succeeded)
        {
            var description = string.Join("; ", result.Errors.Select(e => e.Description));
            return Error.Validation("User.InvalidPassword", description);
        }

        var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        Console.WriteLine($"\n{emailConfirmationToken} has been Created for {command.Email}.\n");
        
        // Build confirmation link (you'll need an endpoint for this)
        var confirmationLink = $"{_config["FrontendUrl"]}/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(emailConfirmationToken)}";
        
        // Send email
        await SendEmailConfirmation(user, confirmationLink);
        
        var (token, expiresAt) = _tokenService.GenerateToken(user, ["user"]);
        
        var (refreshToken, refreshExpiresAt) =
            await _tokenService.GenerateRefreshTokenAsync(user, command.UserAgentData.IpAddress,
                command.UserAgentData.UserAgent, null, ct);
        
        return new AuthResponse(new AuthTokenResponse(token,expiresAt, refreshToken, refreshExpiresAt),
            user.ToAuthResponseUser());
    }

    private async Task SendEmailConfirmation(ApplicationUser user,string confirmationLink)
    {
        // Send via Resend
        await _emailService.SendAsync(
            to: user.Email,
            subject: "Confirm your NimMasir account",
            body: $@"
                <div style='font-family: system-ui, sans-serif; max-width: 480px; margin: 0 auto; color: #111;'>
                    <h2 style='font-size: 24px; margin-bottom: 8px;'>Welcome to PicoNet</h2>
                    <p style='color: #555; line-height: 1.5;'>
                        Thanks for signing up. Please confirm your email address by clicking the button below.
                    </p>
                    <a href='{confirmationLink}' 
                       style='display: inline-block; margin: 16px 0; padding: 12px 24px; background: #0ea5e9; color: white; text-decoration: none; border-radius: 8px; font-weight: 600;'>
                       Confirm Email Address
                    </a>
                    <p style='color: #888; font-size: 13px; margin-top: 24px;'>
                        Didn't work? Copy and paste this link into your browser:<br/>
                        <span style='word-break: break-all; color: #0ea5e9;'>{confirmationLink}</span>
                    </p>
                    <hr style='border: none; border-top: 1px solid #eee; margin: 32px 0;' />
                    <p style='font-size: 12px; color: #aaa;'>
                        If you didn't create an account, you can safely ignore this email.
                    </p>
                </div>"
        );
    }
}