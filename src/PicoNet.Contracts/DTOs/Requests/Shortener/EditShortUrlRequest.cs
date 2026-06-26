using PicoNet.Domain.Enums;

namespace PicoNet.Contracts.DTOs.Requests.Shortener;

public record EditShortUrlRequest
{
    public EditShortUrlRequest(string? originalUrl, string? customAlias, UrlStatus? urlStatus, List<string>? tags, bool isPermanent, DateTime? expiryTime, string? password, string? campaign)
    {
        OriginalUrl = originalUrl;
        CustomAlias = customAlias;
        UrlStatus = urlStatus;
        Tags = tags;
        IsPermanent = isPermanent;
        ExpiryTime = expiryTime;
        Password = password;
        Campaign = campaign;
    }

    public string? OriginalUrl { get; set; }
    public string? CustomAlias { get; set; }
    public UrlStatus? UrlStatus { get; set; }
    public List<string>? Tags { get; set; }
    public bool IsPermanent { get; set; }
    public DateTime? ExpiryTime { get; set; }
    public string? Password { get; set; }
    public string? Campaign { get; set; }
}