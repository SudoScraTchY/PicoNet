using ErrorOr;
using Microsoft.EntityFrameworkCore;
using PicoNet.Application.Events.Commands;
using PicoNet.Application.Features.Redirect.Commands;
using PicoNet.Contracts.DTOs.Cache;
using PicoNet.Contracts.DTOs.Responses.Redirect;
using PicoNet.Domain.Enums;
using PicoNet.Domain.ValueObjects;
using PicoNet.Infrastructure.Cache;
using PicoNet.Infrastructure.Data;
using PicoNet.Infrastructure.Security;
using Wolverine;

namespace PicoNet.Application.Features.Redirect.Handler;

public sealed class RedirectHandler
{
    private readonly PicoNetDbContext _db;
    private readonly IRedirectCacheService _cache;
    private readonly IMessageContext _messageContext;
    private readonly IPasswordHasher _passwordHasher;

    public RedirectHandler(PicoNetDbContext db, IRedirectCacheService cache,  IMessageContext messageContext, IPasswordHasher passwordHasher)
    {
        _db = db;
        _cache = cache;
        _messageContext = messageContext;
        _passwordHasher = passwordHasher;
    }

    public async Task<ErrorOr<RedirectUrlResult>> Handle(RedirectCommand command, CancellationToken ct)
    {
        // 1. Cache hit path
        var cached = await _cache.GetAsync(command.ShortCode, ct);
        if (cached is not null)
        {
            return await ProcessRedirect(cached, command, fromCache: true, ct);
        }

        var shortCode = new ShortCode(command.ShortCode);
        // 2. DB miss path
        var url = await _db.Urls
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.NanoId == shortCode && !u.IsDeleted, ct);

        if (url is null)
            return Error.NotFound("Url.NotFound", $"No URL found for code '{command.ShortCode}'");

        // 3. Populate cache for next time
        var dto = new CachedRedirectDto
        {
            ShortenerId = url.Id,
            OriginalUrl = url.OriginalUrl,
            PasswordHash = url.Password,
            Status = url.Status.ToString(),
            ExpiryTime = url.ExpiryTime,
            ClickCount = url.ClickCount,
            MaxClicks = url.MaxClicks,
        };

        return await ProcessRedirect(dto, command, fromCache: false, ct);
    }

    private async Task<ErrorOr<RedirectUrlResult>> ProcessRedirect(
        CachedRedirectDto dto,
        RedirectCommand command,
        bool fromCache,CancellationToken ct)
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

            if (!_passwordHasher.VerifyPassword(command.Password, dto.PasswordHash))
                return Error.Unauthorized("Url.InvalidPassword", "Incorrect password.");
        }

        // Fire visit event — fire and forget, don't await
        if (!fromCache)
        {
            var hits = await _cache.IncrementHitCountAsync(command.ShortCode, ct);
            var ttl = RedirectCacheTtlPolicy.Resolve(hits);
            await _cache.SetAsync(command.ShortCode, dto, ttl, ct);
        }

        await _messageContext.PublishAsync(new UrlVisitedEvent(
            UrlId: dto.ShortenerId,
            ShortCode: command.ShortCode,
            IpAddress: command.IpAddress,
            UserAgent: command.UserAgent,
            Referrer: command.Referrer,
            VisitedAt: DateTime.UtcNow
        ));

        var result = new RedirectUrlResult(dto.OriginalUrl);
        return result;
    }
}