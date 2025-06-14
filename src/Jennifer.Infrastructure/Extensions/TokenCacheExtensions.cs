using Microsoft.Extensions.Caching.Distributed;

namespace Jennifer.Infrastructure.Extensions;

public static class TokenCacheExtensions
{
    public static async Task SetCacheUserSid(this IDistributedCache cache, string key, string value, TimeSpan? SlidingExpiration = null,
        CancellationToken cancellationToken = default)
    {
        await cache.SetStringAsync(key, value, new DistributedCacheEntryOptions()
        {
            SlidingExpiration = SlidingExpiration,
        }, token: cancellationToken);
    }
}