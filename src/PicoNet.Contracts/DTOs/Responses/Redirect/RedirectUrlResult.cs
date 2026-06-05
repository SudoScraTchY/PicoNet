namespace PicoNet.Contracts.DTOs.Responses.Redirect;

public record RedirectUrlResult
{
    public RedirectUrlResult(string originalUrl)
    {
        OriginalUrl = originalUrl;
    }

    public string OriginalUrl { get; set; }
    public bool IsPasswordProtected { get; set; }
    public int? MaxClicks { get; set; }
}