using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PicoNet.Application.Features.Shortener.Queries;
using PicoNet.Application.Mappings;
using PicoNet.Contracts.DTOs.Responses;
using PicoNet.Contracts.DTOs.Responses.Shortener;
using PicoNet.Contracts.Extensions;
using PicoNet.Infrastructure.Data;

namespace PicoNet.Application.Features.Shortener.Handlers;

public class GetCursorPaginatedUrlsQueryHandler
{
    private readonly PicoNetDbContext _context;
    private readonly ILogger<GetCursorPaginatedUrlsQueryHandler> _logger;
    
    public GetCursorPaginatedUrlsQueryHandler(PicoNetDbContext context, ILogger<GetCursorPaginatedUrlsQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public  async Task<ErrorOr<CursorPaginatedResult<ShortUrlResponse>>> Handle(CursorPaginatedCommand request,CancellationToken ct)
    {
        var query = _context.Urls.AsNoTracking().Where(x => !x.IsDeleted);
        // for future
        // && x.UserId == userId);
        
        var cursorId = PaginationHelper.DecodeCursor(request.Cursor);
        if (cursorId is not null)
            query = query.Where(x => x.Id < cursorId.Value);

        var rawItems = await query
            .OrderByDescending(x => x.CreatedAt)
            .ThenByDescending(x => x.Id)
            .Take(request.PageSize + 1)
            .ToListAsync(ct);  // ← materializes here

        var items = rawItems.Select(x => x.ToShortUrlResponse()).ToList();
        
        if(items.Count == 0)
            return new CursorPaginatedResult<ShortUrlResponse>();
        
        var hasMore = items.Count > request.PageSize;
        var pageItems = items.Take(request.PageSize).ToList();

        return new CursorPaginatedResult<ShortUrlResponse>
        {
            Items = pageItems,
            HasMore = hasMore,
            NextCursor = hasMore ? PaginationHelper.EncodeCursor(pageItems.Last().Id) : null,
            PreviousCursor = request.Cursor
        };
    }
}