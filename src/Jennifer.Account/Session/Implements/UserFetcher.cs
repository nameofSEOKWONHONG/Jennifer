using Jennifer.Account.Data;
using Jennifer.Account.Models;
using Jennifer.Account.Session.Abstracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;

namespace Jennifer.Account.Session.Implements;

public sealed class UserFetcher(HybridCache cache, JenniferReadOnlyDbContext dbContext) : IUserFetcher
{
    private User _cached;
    public async Task<User> FetchAsync(Guid id)
    {
        if (_cached is not null) return _cached;
        
        string userCacheKey = CachingConsts.UserCacheKey(id);
        async ValueTask<User> FetchUserFromDatabase(CancellationToken token) =>
            await dbContext.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken: token);
        _cached = await cache.GetOrCreateAsync(userCacheKey, FetchUserFromDatabase);
        
        return _cached;
    }
}