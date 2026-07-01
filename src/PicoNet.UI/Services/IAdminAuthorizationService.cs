namespace PicoNet.UI.Services;

public interface IAdminAuthorizationService
{
    Task<bool> IsAdministratorAsync();
}
