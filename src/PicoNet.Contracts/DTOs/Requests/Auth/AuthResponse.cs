namespace PicoNet.Contracts.DTOs.Requests.Auth;

public record AuthResponse
{
    public string AccessToken { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
    public string Email { get; init; } = string.Empty;
}