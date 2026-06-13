namespace PicoNet.Contracts.DTOs.Cache;

public sealed record CachedRedirectDto
{
    public Guid ShortenerId { get; set; }
    public string OriginalUrl { get; init; } = string.Empty;
    public string? PasswordHash { get; init; }   // null = no password
    public string Status { get; init; } = string.Empty;
    public DateTime? ExpiryTime { get; init; }
    public long ClickCount { get; init; }
    public int? MaxClicks { get; init; }
}