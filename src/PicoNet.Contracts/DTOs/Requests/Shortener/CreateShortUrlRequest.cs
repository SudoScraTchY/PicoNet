namespace PicoNet.Contracts.DTOs.Requests.Shortener;

public record CreateShortUrlRequest()
{
    public string OriginalUrl { get; set; } = string.Empty;
    public string? CustomAlias { get; set; }
    public List<string>? Tags { get; set; }
    public string Password { get; set; } = string.Empty;
    public int MaxClicks { get; set; }
    public DateTime? ExpiryTime { get; set; }
    public string? Campaign { get; set; }
}