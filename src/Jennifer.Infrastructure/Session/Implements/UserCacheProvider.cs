using Jennifer.Infrastructure.Database;
using Jennifer.Infrastructure.Session.Abstracts;
using Jennifer.Infrastructure.Session.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Hybrid;

namespace Jennifer.Infrastructure.Session.Implements;

public sealed class UserCacheProvider(IDistributedCache cache,
    HybridCache hybridCache,
    JenniferReadOnlyDbContext dbContext) : IUserCacheProvider
{
    private UserCacheResult _cached;
    public async Task<UserCacheResult> GetAsync(string sid)
    {
        if (_cached is not null) return _cached;
        
        var value = await cache.GetStringAsync(CachingConsts.SidCacheKey(sid));
        async ValueTask<UserCacheResult> FetchFromDatabase(CancellationToken token) =>
            await dbContext.Users.Where(m => m.Id == Guid.Parse(value))
                .Select(m => new UserCacheResult
                {
                    Id = m.Id,
                    UserName = m.UserName,
                    Email = m.Email,
                    PhoneNumber = m.PhoneNumber,
                    ConcurrencyStamp = m.ConcurrencyStamp
                })
                .FirstAsync(cancellationToken: token);
        
        _cached = await hybridCache.GetOrCreateAsync(CachingConsts.UserCacheKey(value), FetchFromDatabase);
        
        return _cached;
    }

    public async Task ClearAsync(string sid)
    {
        var value = await cache.GetStringAsync(CachingConsts.SidCacheKey(sid));
        var userKey = CachingConsts.UserCacheKey(value);
        await hybridCache.RemoveAsync(userKey);
        await cache.RemoveAsync(CachingConsts.SidCacheKey(sid));
    }
}