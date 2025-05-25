using eXtensionSharp;
using Jennifer.Account.Data;
using Jennifer.Account.Models;
using Jennifer.Account.Session.Abstracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace Jennifer.Account.Session.Implements;

public sealed class UserFetcher(IDistributedCache cache, JenniferReadOnlyDbContext dbContext) : IUserFetcher
{
    private User _cached;
    public async Task<User> FetchAsync(Guid id)
    {
        if (_cached.xIsNotEmpty()) return _cached;
        
        var cacheValue = await cache.GetStringAsync(id.ToString());
        if (cacheValue.xIsEmpty())
        {
            var exists = await dbContext.Users.FirstOrDefaultAsync(m => m.Id == id);
            if(exists.xIsEmpty()) throw new Exception("User not found.");

            await cache.SetStringAsync(id.ToString(), exists.xSerialize());
            
            _cached = exists;
        }

        return _cached;
    }
}