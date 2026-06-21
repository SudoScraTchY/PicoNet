using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PicoNet.Application.Features.Shortener.Queries;
using PicoNet.Application.Mappings;
using PicoNet.Contracts.DTOs.Responses.Shortener;
using PicoNet.Infrastructure.Data;

namespace PicoNet.Application.Features.Shortener.Handlers;

public sealed class GetShortenedUrlQueryHandler
{
    private readonly PicoNetDbContext _context;
    private readonly ILogger<GetShortenedUrlQueryHandler> _logger;

    public GetShortenedUrlQueryHandler(PicoNetDbContext context, ILogger<GetShortenedUrlQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ErrorOr<ShortUrlResponse>> Handle(GetShortUrlByIdQuery query, CancellationToken ct)
    {
        var result = await _context.Urls.AsNoTracking()
            .Where(x => x.UserId == query.UserContext.UserId && !x.IsDeleted && x.Id == query.UrlId)
            .Select(x=>x.ToShortUrlResponse())
            .FirstOrDefaultAsync(ct);

        return result != null ? result : Error.NotFound();
    }
}