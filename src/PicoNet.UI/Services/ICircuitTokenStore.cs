namespace PicoNet.UI.Services;

public interface ICircuitTokenStore
{
    string? AccessToken { get; set; }
    string? RefreshToken { get; set; }
    DateTimeOffset AccessTokenExpiresAt { get; set; }
    DateTimeOffset RefreshTokenExpiresAt { get; set; }
}