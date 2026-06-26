using PicoNet.UI.Models;

namespace PicoNet.UI.Services;

public class CurrentUserService : ICurrentUserService
{
    private UserSession? _user;

    public UserSession? User => _user;
    public bool IsAuthenticated => _user != null;

    public void SetUser(UserSession user) => _user = user;
    public void Clear() => _user = null;
}