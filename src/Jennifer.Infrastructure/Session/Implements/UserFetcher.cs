using Jennifer.Account.Session.Abstracts;
using Jennifer.Domain.Account;
using Jennifer.Domain.Database;
using Jennifer.Infrastructure.Session.Abstracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;

namespace Jennifer.Infrastructure.Session.Implements;

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