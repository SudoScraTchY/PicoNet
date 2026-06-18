using ErrorOr;
using PicoNet.Contracts.DTOs.Responses.Redirect;

namespace PicoNet.UI.ApiClients.Interfaces;

public interface IRedirectClient
{
    Task<ErrorOr<RedirectUrlResult?>> ResolveAsync(string shortCode, string? password);
}