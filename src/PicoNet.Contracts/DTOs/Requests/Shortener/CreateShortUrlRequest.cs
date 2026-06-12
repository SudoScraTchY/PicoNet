using System.ComponentModel.DataAnnotations;

namespace PicoNet.Contracts.DTOs.Requests.Shortener;

public record CreateShortUrlRequest()
{
    [Required(ErrorMessage = "URL is required.")]
    [Url(ErrorMessage = "Must be a valid absolute URL.")]
    public string OriginalUrl { get; set; } = string.Empty;

    [RegularExpression(@"^[a-zA-Z0-9_-]{4,20}$",
        ErrorMessage = "Alias must be 4–20 characters: letters, numbers, - or _")]
    public string? CustomAlias { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Max clicks cannot be negative.")]
    public int MaxClicks { get; set; }
    public List<string>? Tags { get; set; }
    [MaxLength(2048, ErrorMessage = "Password cannot be longer than 2048 characters.")]
    public string Password { get; set; } = string.Empty;
    public DateTime? ExpiryTime { get; set; }
    [MaxLength(512, ErrorMessage = "Campaign cannot be longer than 512 characters.")]
    public string? Campaign { get; set; }
}