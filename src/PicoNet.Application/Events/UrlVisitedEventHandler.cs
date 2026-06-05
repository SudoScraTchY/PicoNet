using Microsoft.EntityFrameworkCore;
using PicoNet.Contracts.Events;
using PicoNet.Domain.Events;
using PicoNet.Infrastructure.Data;

namespace PicoNet.Application.Events;

public sealed class UrlVisitedEventHandler
{
    private readonly PicoNetDbContext _db;

    public UrlVisitedEventHandler(PicoNetDbContext db) => _db = db;

    public async Task Handle(UrlVisitedEvent evt, CancellationToken ct)
    {
        var url = await _db.Urls
            .Include(u => u.Visits)
            .FirstOrDefaultAsync(u => u.NanoId == evt.ShortCode, ct);

        if (url is null) return; // already deleted, just skip

        url.RecordVisit(evt.IpAddress, evt.UserAgent, evt.Referrer, country: null);
        await _db.SaveChangesAsync(ct);
    }
}