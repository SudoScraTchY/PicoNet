using PicoNet.UI.Models;

namespace PicoNet.UI.Services;

public interface ICurrentUserService
{
    UserSession? User { get; }
    bool IsAuthenticated { get; }
    void SetUser(UserSession user);
    void Clear();
}