using ErrorOr;
using Microsoft.EntityFrameworkCore;
using PicoNet.Application.Events.Commands;
using PicoNet.Application.Features.Redirect.Commands;
using PicoNet.Contracts.DTOs.Cache;
using PicoNet.Contracts.DTOs.Responses.Redirect;
using PicoNet.Domain.Enums;
using PicoNet.Infrastructure.Cache;
using PicoNet.Infrastructure.Data;
using Wolverine;

namespace PicoNet.Application.Features.Redirect.Handler;

public sealed class RedirectHandler
{
    private readonly PicoNetDbContext _db;
    private readonly IRedirectCacheService _cache;
    private readonly IMessageBus _bus;

    public RedirectHandler(PicoNetDbContext db, IRedirectCacheService cache, IMessageBus bus)
    {
        _db = db;
        _cache = cache;
        _bus = bus;
    }

    public async Task<ErrorOr<RedirectUrlResult>> Handle(RedirectCommand command, CancellationToken ct)
    {
        // 1. Cache hit path
        var cached = await _cache.GetAsync(command.ShortCode, ct);
        if (cached is not null)
        {
            return await ProcessRedirect(cached, command, fromCache: true, ct: ct);
        }

        //var shortCode = new ShortCode(command.ShortCode);
        // 2. DB miss path
        var url = await _db.Urls
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.NanoId.Value == command.ShortCode && !u.IsDeleted, ct);

        if (url is null)
            return Error.NotFound("Url.NotFound", $"No URL found for code '{command.ShortCode}'");

        // 3. Populate cache for next time
        var dto = new CachedRedirectDto
        {
            OriginalUrl = url.OriginalUrl,
            PasswordHash = url.Password,
            Status = url.Status.ToString(),
            ExpiryTime = url.ExpiryTime,
            ClickCount = url.ClickCount,
            MaxClicks = url.MaxClicks
        };

        return await ProcessRedirect(dto, command, fromCache: false, ct: ct);
    }

    private async Task<ErrorOr<RedirectUrlResult>> ProcessRedirect(
        CachedRedirectDto dto,
        RedirectCommand command,
        bool fromCache,
        CancellationToken ct)
    {
        // Status checks
        if (dto.Status != nameof(UrlStatus.Active))
            return Error.Conflict("Url.Inactive", "This URL is no longer active.");

        if (dto.ExpiryTime.HasValue && DateTime.UtcNow > dto.ExpiryTime.Value)
            return Error.Conflict("Url.Expired", "This URL has expired.");

        if (dto.MaxClicks > 0 && dto.ClickCount >= dto.MaxClicks)
            return Error.Conflict("Url.MaxClicks", "This URL has reached its click limit.");
        
        // Password check
        if (!string.IsNullOrEmpty(dto.PasswordHash))
        {
            if (string.IsNullOrEmpty(command.Password))
                return Error.Unauthorized("Url.PasswordRequired", "This URL is password protected.");

            // TODO: use proper hash comparison (BCrypt or similar) when you add auth
            if (command.Password != dto.PasswordHash)
                return Error.Unauthorized("Url.InvalidPassword", "Incorrect password.");
        }

        // Fire visit event — fire and forget, don't await
        await _bus.PublishAsync(new UrlVisitedEvent(
            dto.ShortenerId,
            fromCache,
            dto,
            RedirectCommand: command,
            VisitedAt: DateTime.UtcNow
        ));

        return new RedirectUrlResult(dto.OriginalUrl);
    }
}