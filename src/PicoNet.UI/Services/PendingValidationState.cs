using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace PicoNet.UI.Services;

public sealed class PendingValidationState(ProtectedSessionStorage storage)
{
    private const string EmailKey = "piconet.pending.validation.email";

    public async Task SetEmailAsync(string email) => await storage.SetAsync(EmailKey, email);
    public async Task<string?> GetEmailAsync()
    {
        var result = await storage.GetAsync<string>(EmailKey);
        return result.Success ? result.Value : null;
    }
    public async Task ClearAsync() => await storage.DeleteAsync(EmailKey);
}