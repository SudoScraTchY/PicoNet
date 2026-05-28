using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PicoNet.Contracts.DTOs.Responses;
using PicoNet.Infrastructure.Data;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using PicoNet.Application.Mappings;
using PicoNet.Contracts.DTOs.Requests;
using PicoNet.Contracts.DTOs.Responses.Shortner;

namespace PicoNet.Application.Features.Shortener.Queries;

public class GetPaginatedUrlsQuery
{
    private readonly PicoNetDbContext _context;
    private readonly ILogger<GetPaginatedUrlsQuery> _logger;
    
    public GetPaginatedUrlsQuery(PicoNetDbContext context, ILogger<GetPaginatedUrlsQuery> logger)
    {
        _context = context;
        _logger = logger;
    }

    public  async Task<ErrorOr<PaginatedResult<ShortUrlResponse>>> GetPaginatedUrls(PaginatedRequestDto request,CancellationToken ct)
    {
        var query = _context.Urls.AsNoTracking().Where(x => !x.IsDeleted);

        var totalCountQuery = await query.CountAsync(ct);
        if (totalCountQuery < 1)
        {
            return new PaginatedResult<ShortUrlResponse>()
            {
                PageSize = request.PageSize,
                PageNumber = request.PageNumber,
                TotalCount = totalCountQuery,
                TotalPages = totalCountQuery / request.PageSize + 1,
                Items = null
            };
        }
        var items = await query
            .Skip((request.PageNumber-1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => x.MapUrlResponse())
            .ToListAsync(cancellationToken: ct);

        return new PaginatedResult<ShortUrlResponse>()
        {
            PageSize = request.PageSize,
            PageNumber = request.PageNumber,
            TotalCount = totalCountQuery,
            TotalPages = totalCountQuery / request.PageSize + 1,
            Items = items
        };
    }
}