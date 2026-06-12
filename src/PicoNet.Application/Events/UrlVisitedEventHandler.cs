using Microsoft.EntityFrameworkCore;
using PicoNet.Contracts.Events;
using PicoNet.Domain.Entities;
using PicoNet.Infrastructure.Data;

namespace PicoNet.Application.Events;

public sealed class UrlVisitedEventHandler
{
    private readonly PicoNetDbContext _db;

    public UrlVisitedEventHandler(PicoNetDbContext db) => _db = db;

    public async Task Handle(UrlVisitedEvent evt, CancellationToken ct)
    {
        var url = await _db.Urls
            .FirstOrDefaultAsync(u => u.Id == evt.UrlId, ct);

        if (url is null)
            return;

        url.IncrementClickCount();
        
        var visit = UrlVisit.Create(url.Id, evt.IpAddress, evt.UserAgent, evt.Referrer, null);
        await _db.Set<UrlVisit>().AddAsync(visit, ct);

        await _db.SaveChangesAsync(ct);
    }
}