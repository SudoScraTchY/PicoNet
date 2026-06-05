using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using PicoNet.Contracts.DTOs.Cache;

namespace PicoNet.Infrastructure.Cache;

public sealed class RedirectCacheService : IRedirectCacheService
{
    private readonly IDistributedCache _cache;
    private static string Key(string code) => $"redir-{code}";
    private static string HitKey(string code) => $"redir-hits-{code}";

    public RedirectCacheService(IDistributedCache cache) => _cache = cache;

    public async Task<CachedRedirectDto?> GetAsync(string shortCode, CancellationToken ct = default)
    {
        var bytes = await _cache.GetAsync(Key(shortCode), ct);
        return bytes is null ? null : JsonSerializer.Deserialize<CachedRedirectDto>(bytes);
    }

    public async Task SetAsync(string shortCode, CachedRedirectDto dto, TimeSpan ttl, CancellationToken ct = default)
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(dto);
        await _cache.SetAsync(Key(shortCode), bytes,
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = ttl }, ct);
    }

    public async Task RemoveAsync(string shortCode, CancellationToken ct = default)
        => await _cache.RemoveAsync(Key(shortCode), ct);

    public async Task<long> IncrementHitCountAsync(string shortCode, CancellationToken ct = default)
    {
        // We'll use a separate counter key for adaptive TTL
        // IDistributedCache doesn't have increment — we store a simple counter
        var key = HitKey(shortCode);
        var bytes = await _cache.GetAsync(key, ct);
        var count = bytes is null ? 1 : BitConverter.ToInt64(bytes) + 1;
        await _cache.SetAsync(key, BitConverter.GetBytes(count),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(3) }, ct);
        return count;
    }
}