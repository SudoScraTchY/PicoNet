using PicoNet.Contracts.DTOs.Responses.Shortener;
using PicoNet.Domain.Entities;
using PicoNet.Domain.ValueObjects;
using Riok.Mapperly.Abstractions;

namespace PicoNet.Application.Mappings;

[Mapper]
public static partial class UrlMapper
{
    private static string MapShortCode(ShortCode shortCode) => shortCode.Value; 
    
    [MapProperty(nameof(ShortenedUrl.NanoId), nameof(ShortUrlResponse.ShortCode))]
    public static partial ShortUrlResponse ToShortUrlResponse(this ShortenedUrl url);

    private static List<string>? MapStringList(string? tags) =>
        tags?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();
}