using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PicoNet.Application.Features.Shortener.Commands;
using PicoNet.Application.Mappings;
using PicoNet.Contracts.DTOs.Responses.Shortener;
using PicoNet.Domain.Enums;
using PicoNet.Infrastructure.Cache;
using PicoNet.Infrastructure.Data;

namespace PicoNet.Application.Features.Shortener.Handlers;

public class EditShortUrlHandler
{
    private readonly PicoNetDbContext _context;
    private readonly IRedirectCacheService _redirectCacheService;
    private readonly ILogger<EditShortUrlHandler> _logger;

    public EditShortUrlHandler(PicoNetDbContext context,IRedirectCacheService redirectCacheService, ILogger<EditShortUrlHandler> logger)
    {
        _context = context;
        _redirectCacheService = redirectCacheService;
        _logger = logger;
    }

    public async Task<ErrorOr<ShortUrlResponse>> Handle(EditShortUrlCommand request, CancellationToken ct)
    {
        var shortenedUrl = await _context.Urls.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.ShortenedUrlId, cancellationToken: ct);

        if (shortenedUrl is null || shortenedUrl.IsDeleted)
        {
            return Error.NotFound();
        }

        if (shortenedUrl.Status == UrlStatus.Suspended)
        {
            return Error.Forbidden();
        }

        if (shortenedUrl.Status == UrlStatus.Expired)
        {
            return Error.Failure();
        }

        shortenedUrl.Edit(request.OriginalUrl, request.CustomAlias, request.IsPermanent,
            request.Password, request.Campaign, request.UrlStatus,
            request.Tags != null ? string.Join(',', request.Tags) : null
            , request.ExpiryTime);
        
        _context.Urls.Update(shortenedUrl);
        await _redirectCacheService.RemoveAsync(shortenedUrl.NanoId.Value,ct);
        await _context.SaveChangesAsync(ct);
        
        return shortenedUrl.ToShortUrlResponse();
    }
}