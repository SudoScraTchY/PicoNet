using PicoNet.Application.Features.Auth.Commands;
using PicoNet.Contracts.DTOs.Responses.Auth;
using ErrorOr;
using Microsoft.AspNetCore.Identity;
using PicoNet.Infrastructure.Identity;
using PicoNet.Infrastructure.Identity.Interfaces;

namespace PicoNet.Application.Features.Auth.Handler;

public sealed class RefreshHandler
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;

    public RefreshHandler(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    public async Task<ErrorOr<AuthResponse>> Handle(RefreshCommand command, CancellationToken cancellationToken)
    {
        
        if (command.UserContext?.UserId != null)
        {
            
        }
        else
        {
            
        }

        return new ErrorOr<AuthResponse>();
    }
}