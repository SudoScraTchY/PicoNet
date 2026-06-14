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
        await _db.Urls.AsNoTracking()
            .ExecuteUpdateAsync(
                x => x.SetProperty(
                    p => p.ClickCount, n => n.ClickCount + 1), ct);

        var visit = UrlVisit.Create(evt.UrlId, evt.IpAddress, 
            evt.UserAgent, evt.Referrer, null);
        
        await _db.Set<UrlVisit>().AddAsync(visit, ct);

        await _db.SaveChangesAsync(ct);
    }
}