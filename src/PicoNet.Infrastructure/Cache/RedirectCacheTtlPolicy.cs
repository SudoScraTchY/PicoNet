namespace PicoNet.Infrastructure.Cache;

public static class RedirectCacheTtlPolicy
{
    private static readonly TimeSpan Min = TimeSpan.FromMinutes(1);
    private static readonly TimeSpan Default = TimeSpan.FromMinutes(15);
    private static readonly TimeSpan Max = TimeSpan.FromHours(3);

    // Thresholds: after N hits within the window, promote to next tier
    public static TimeSpan Resolve(long hitCount) => hitCount switch
    {
        < 10  => Min,
        < 50  => Default,
        _     => Max
    };
}