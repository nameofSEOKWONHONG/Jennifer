using Jennifer.Infrastructure.Database;
using Jennifer.Infrastructure.Session.Abstracts;
using Jennifer.Infrastructure.Session.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Hybrid;

namespace Jennifer.Infrastructure.Session.Implements;

public sealed class UserOptionCacheProvider(JenniferReadOnlyDbContext dbContext,
    IDistributedCache cache,
    HybridCache hybridCache): IUserOptionCacheProvider
{
    private UserOptionCacheResult[] _cached;
    public async Task<UserOptionCacheResult[]> GetAsync(string sid)
    {
        if (_cached is not null) return _cached;
        
        var userId = await cache.GetStringAsync(CachingConsts.SidCacheKey(sid));
        async ValueTask<UserOptionCacheResult[]> FetchFromDatabase(CancellationToken token) =>
            await dbContext.UserOptions
                .Where(m => m.UserId == Guid.Parse(userId))
                .Select(m => new UserOptionCacheResult(m.Type, m.Value))
                .ToArrayAsync(cancellationToken: token);
        
        _cached = await hybridCache.GetOrCreateAsync(CachingConsts.UserOptionCacheKey(userId), FetchFromDatabase);
        
        return _cached;
    }

    public async Task ClearAsync(string sid)
    {
        var key = CachingConsts.UserOptionCacheKey(sid);
        await hybridCache.RemoveAsync(key);
    }
}