using System.Text.RegularExpressions;
using eXtensionSharp;
using Jennifer.Domain.Account;
using Jennifer.Infrastructure.Database;
using Jennifer.Infrastructure.Session.Abstracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Hybrid;

namespace Jennifer.Infrastructure.Session.Implements;

public sealed class UserFetcher(IDistributedCache cache,
    HybridCache hybridCache,
    JenniferReadOnlyDbContext dbContext) : IUserFetcher
{
    private User _cached;
    public async Task<User> FetchAsync(string sid)
    {
        if (_cached is not null) return _cached;
        
        var value = await cache.GetStringAsync(CachingConsts.SidCacheKey(sid));
        async ValueTask<User> FetchUserFromDatabase(CancellationToken token) =>
            await dbContext.Users.FirstOrDefaultAsync(u => u.Id == Guid.Parse(value), cancellationToken: token);
        
        _cached = await hybridCache.GetOrCreateAsync(CachingConsts.UserCacheKey(value), FetchUserFromDatabase);
        
        return _cached;
    }
}