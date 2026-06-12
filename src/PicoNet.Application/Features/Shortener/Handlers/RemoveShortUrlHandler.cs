using ErrorOr;
using Microsoft.EntityFrameworkCore;
using PicoNet.Application.Features.Shortener.Commands;
using PicoNet.Infrastructure.Data;

namespace PicoNet.Application.Features.Shortener.Handlers;

public class RemoveShortUrlHandler
{
    private readonly PicoNetDbContext _dbContext;

    public RemoveShortUrlHandler(PicoNetDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<bool>> Handle(RemoveShortUrlCommand command, CancellationToken ct)
    {
        var url = await _dbContext.Urls.AsNoTracking()
            .FirstOrDefaultAsync(x=>x.Id == command.UrlId && !x.IsDeleted , ct);

        if (url is null)
            return Error.NotFound();

        url.SoftDelete();
        url.Modified();
        return await _dbContext.SaveChangesAsync(ct) > 0;
        
    }
}