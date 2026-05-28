using PicoNet.Contracts.DTOs.Responses.Shortner;
using PicoNet.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace PicoNet.Application.Mappings;

[Mapper]
public partial class UrlMapper
{
    //public partial ShortUrlResponse ToShortUrlResponse(ShortenedUrl shortenedUrl);
}

public static class UrlMapperExtensions
{
    public static ShortUrlResponse MapUrlResponse(this ShortenedUrl shortenedUrl) =>
        new ShortUrlResponse(shortenedUrl.Id, shortenedUrl.NanoId.Value, shortenedUrl.OriginalUrl,
            shortenedUrl.CustomAlias
            , shortenedUrl.CreatedAt, shortenedUrl.ExpiryTime, shortenedUrl.Status,
            shortenedUrl.Tags?.Split(',').ToList());
}