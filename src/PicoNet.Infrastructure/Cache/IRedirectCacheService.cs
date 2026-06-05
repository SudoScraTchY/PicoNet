// PicoNet.Infrastructure/Cache/IRedirectCacheService.cs

using PicoNet.Contracts.DTOs.Cache;

namespace PicoNet.Infrastructure.Cache;

public interface IRedirectCacheService
{
    Task<CachedRedirectDto?> GetAsync(string shortCode, CancellationToken ct = default);
    Task SetAsync(string shortCode, CachedRedirectDto dto, TimeSpan ttl, CancellationToken ct = default);
    Task RemoveAsync(string shortCode, CancellationToken ct = default);
    Task<long> IncrementHitCountAsync(string shortCode, CancellationToken ct = default);
}