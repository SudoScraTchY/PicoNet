# Caching (Redis) and Redirect Fast-Path

Overview
- Redis is used as a distributed cache for redirect lookups to provide a fast-path for frequently accessed short codes.
- The cache contract is `PicoNet.Infrastructure.Cache.IRedirectCacheService` and the implementation is `RedirectCacheService`.

Cache keys and DTOs
- Cache keys follow these patterns (implementation in `RedirectCacheService`):
  - Primary key: `redir-{shortCode}`
  - Hit count key: `redir-hits-{shortCode}`
- Cached payload is `CachedRedirectDto` (check `PicoNet.Contracts.DTOs.Cache`) and contains fields required to do a redirect without hitting the DB (OriginalUrl, ShortenerId, PasswordHash, MaxClicks, ExpiryTime, etc.).

Flow (as implemented in `RedirectHandler`)
1. Controller receives redirect request -> builds `RedirectCommand` and dispatches via Wolverine message bus.
2. `RedirectHandler` first tries `_cache.GetAsync(command.ShortCode)`.
   - If cache hit: process redirect path using cached data (fast-path).
   - If cache miss: query DB for `ShortenedUrl` -> populate cache with `SetAsync(shortCode, dto, ttl)` for next hits.
3. On successful redirect, a `UrlVisitedEvent` is raised and handled by `UrlVisitedEventHandler` which increments click counts and persists a `UrlVisit` record.

Dynamic TTL management
- There is a place in the code that sets TTLs when calling `SetAsync` in `RedirectHandler`. The system keeps a separate hit counter (`IncrementHitCountAsync`) and the service can increment the TTL for very hot links ("high frequency") and reduce TTL for low frequency links.
- The `RedirectCacheService` uses `IDistributedCache` operations and stores both the serialized cached entry and a numeric hit counter. Adjust TTL strategy here to suit your usage patterns.

Cache invalidation
- When a short link is edited or deleted, use `IRedirectCacheService.RemoveAsync(shortCode)` to remove the cached entry. Search for usages of `RemoveAsync` in application handlers (handlers that update or delete links should call this).

Config and connection
- API registers the Redis distributed cache in `PicoNet.Api/Program.cs` via `builder.AddRedisDistributedCache(connectionName: "piconet-cache")`.
- UI expects the Redis connection string via `ConnectionStrings__piconet-cache` or `REDIS_CONNECTION_STRING` as a fallback.

Operational tips
- For high throughput / hot links, consider:
  - Increasing TTL for hot links (update TTL after hit counting threshold exceeded).
  - Using Redis memory eviction policies or limiting key TTLs to avoid unbounded memory growth.
  - Monitoring `redir-hits-{shortCode}` counters to feed TTL adjustments.

