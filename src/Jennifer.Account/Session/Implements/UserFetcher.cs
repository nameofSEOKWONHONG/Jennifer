using eXtensionSharp;
using Jennifer.Account.Models;
using Jennifer.Account.Session.Abstracts;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Hybrid;

namespace Jennifer.Account.Session.Implements;

public sealed class UserFetcher(IDistributedCache cache) : IUserFetcher
{
    private User _cached;
    public async Task<User> FetchAsync(Guid id)
    {
        if (_cached.xIsNotEmpty()) return _cached;
        
        var cacheValue = await cache.GetStringAsync(id.ToString());
        if (cacheValue.xIsEmpty()) return null;
        
        _cached = cacheValue.xDeserialize<User>();

        return _cached;
    }
}