using ErrorOr;
using PicoNet.Contracts.DTOs.Requests.Auth;

namespace PicoNet.UI.ApiClients.Interfaces;

public interface IBlazorInternalApi
{
    Task<ErrorOr<HttpResponseMessage>> Login(LoginRequest model);
}