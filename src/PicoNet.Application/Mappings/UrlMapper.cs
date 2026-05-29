using PicoNet.Contracts.DTOs.Responses.Shortener;
using PicoNet.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace PicoNet.Application.Mappings;

[Mapper]
public static partial class UrlMapper
{
    public static partial ShortUrlResponse ToShortUrlResponse(this ShortenedUrl url);

    private static List<string>? MapStringList(string? tags) =>
        tags?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();
}