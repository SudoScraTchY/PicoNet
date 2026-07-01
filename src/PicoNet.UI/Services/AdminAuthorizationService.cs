using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace PicoNet.UI.Services;

public class AdminAuthorizationService : IAdminAuthorizationService
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public AdminAuthorizationService(AuthenticationStateProvider authenticationStateProvider)
    {
        _authenticationStateProvider = authenticationStateProvider;
    }

    public async Task<bool> IsAdministratorAsync()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (!user.Identity?.IsAuthenticated ?? false)
            return false;

        return user.IsInRole("Admin") || user.IsInRole("Administrator");
    }
}
