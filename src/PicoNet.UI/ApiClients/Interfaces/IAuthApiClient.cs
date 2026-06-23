using PicoNet.Contracts.DTOs.Requests.Auth;
using PicoNet.Contracts.DTOs.Responses.Auth;
using ErrorOr;

namespace PicoNet.UI.ApiClients.Interfaces;

public interface IAuthApiClient
{
    Task<ErrorOr<AuthResponse>> LoginAsync(LoginRequest command, CancellationToken ct);
    Task<ErrorOr<AuthResponse>> RegisterAsync(RegisterRequest command, CancellationToken ct);
    Task<ErrorOr<AuthResponse>> ValidateEmailAsync(ValidateRegistrationRequest command, CancellationToken ct);
}