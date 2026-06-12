using Microsoft.EntityFrameworkCore;
using PicoNet.Application.Events.Commands;
using PicoNet.Domain.Entities;
using PicoNet.Infrastructure.Cache;
using PicoNet.Infrastructure.Data;

namespace PicoNet.Application.Events.Handler;

public sealed class UrlVisitedEventHandler
{
    private readonly PicoNetDbContext _db;
    private readonly IRedirectCacheService _cache;

    public UrlVisitedEventHandler(PicoNetDbContext db, IRedirectCacheService cache)
    {
        _db = db;
        _cache = cache;
    }

    public async Task Handle(UrlVisitedEvent evt, CancellationToken ct)
    {
        var url = await _db.Urls
            .FirstOrDefaultAsync(u => u.Id == evt.UrlId, ct);

        if (url is null)
            return;

        url.IncrementClickCount();
        
        if (!evt.FromCache)
        {
            var hits = await _cache.IncrementHitCountAsync(evt.RedirectCommand.ShortCode, ct);
            var ttl = RedirectCacheTtlPolicy.Resolve(hits);
            await _cache.SetAsync(evt.RedirectCommand.ShortCode, evt.CachedRedirect, ttl, ct);
        }

        
        var visit = UrlVisit.Create(url.Id, evt.RedirectCommand.IpAddress, 
            evt.RedirectCommand.UserAgent, evt.RedirectCommand.Referrer, null);
        
        await _db.Set<UrlVisit>().AddAsync(visit, ct);

        await _db.SaveChangesAsync(ct);
    }
}