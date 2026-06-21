using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PicoNet.Application.Features.Shortener.Queries;
using PicoNet.Application.Mappings;
using PicoNet.Contracts.DTOs.Requests;
using PicoNet.Contracts.DTOs.Responses;
using PicoNet.Contracts.DTOs.Responses.Shortener;
using PicoNet.Infrastructure.Data;

namespace PicoNet.Application.Features.Shortener.Handlers;

public class GetOffsetPaginatedUrlQueryHandler
{
    private readonly PicoNetDbContext _context;
    private readonly ILogger<GetOffsetPaginatedUrlQueryHandler> _logger;
    
    public GetOffsetPaginatedUrlQueryHandler(PicoNetDbContext context, ILogger<GetOffsetPaginatedUrlQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public  async Task<ErrorOr<OffsetPaginatedResult<ShortUrlResponse>>> Handle(OffsetPaginatedCommand request,CancellationToken ct)
    {
        var query = _context.Urls.AsNoTracking().Where(x => x.UserId == request.UserContext.UserId && !x.IsDeleted);
        
        var totalCountQuery = await query.CountAsync(ct);
        if (totalCountQuery < 1)
        {
            return new OffsetPaginatedResult<ShortUrlResponse>()
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
            .Select(x => x.ToShortUrlResponse())
            .ToListAsync(cancellationToken: ct);

        return new OffsetPaginatedResult<ShortUrlResponse>()
        {
            PageSize = request.PageSize,
            PageNumber = request.PageNumber,
            TotalCount = totalCountQuery,
            TotalPages = totalCountQuery / request.PageSize + 1,
            Items = items
        };
    }
}
