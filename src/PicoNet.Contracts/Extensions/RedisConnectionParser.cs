namespace PicoNet.Contracts.Extensions;

public static class RedisConnectionParser
{
    public static string ParseRenderRedisUrl(string? redisUrl)
    {
        if (string.IsNullOrWhiteSpace(redisUrl))
            throw new ArgumentException("Redis URL cannot be null or empty", nameof(redisUrl));

        // Handle redis://host:port format
        if (redisUrl.StartsWith("redis://", StringComparison.OrdinalIgnoreCase))
        {
            var uri = new Uri(redisUrl);
            return $"{uri.Host}:{uri.Port}";
        }

        // Already in host:port format
        return redisUrl;
    }
}