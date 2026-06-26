using PicoNet.Domain.Enums;

namespace PicoNet.Contracts.DTOs.Responses.Shortener;

public record ShortUrlResponse()
{
    public Guid Id { get; init; }
    public string ShortCode {get; init;}
    public string OriginalUrl { get; init; }
    public string? CustomAlias { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ExpiresAt { get; init; }
    public UrlStatus Status { get; init; }
    public List<string>? Tags  { get; init; }
    public int MaxClicks  { get; init; }
    public long ClickCount { get; init; }
    public bool IsPermanent { get; set; }
    public string? Password { get; init; }
    public string? Campaign { get; init; }
    public DateTime? LastAccessedAt { get; init; }
    public string? AllowedDomains { get; init; }
}

public record ShortUrlAnalyticsResponse
{
    public ShortUrlResponse ShortUrlResponse;
    public UrlAnalyticsResponse UrlAnalytics = new UrlAnalyticsResponse();
}

public record UrlAnalyticsResponse
{
    public IReadOnlyList<UrlClicksDataPoint> UrlClicksByDay = new List<UrlClicksDataPoint>();
    public IReadOnlyList<UrlClicksDataPoint> UrlClicksByMonth = new List<UrlClicksDataPoint>();
    public IReadOnlyList<UrlClicksDataPoint> UrlClicksByYear = new List<UrlClicksDataPoint>();
    public IReadOnlyList<UrlTopReferrers> TopReferrers = new List<UrlTopReferrers>();
    public IReadOnlyList<UrlTopCountries> TopCountries = new List<UrlTopCountries>();
}

public record UrlClicksDataPoint
{
    public DateOnly Date { get; init; }
    public long Clicks { get; init; }
}

public record UrlTopReferrers
{
    public string Referrer { get; init; }
    public long Count { get; init; }
}

public record UrlTopCountries
{
    public string Country { get; init; }
    public long Count { get; init; }
}