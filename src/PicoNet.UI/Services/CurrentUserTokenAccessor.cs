namespace PicoNet.UI.Services;

public sealed class CurrentUserTokenAccessor
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? AccessTokenExpiresAt { get; set; }
}