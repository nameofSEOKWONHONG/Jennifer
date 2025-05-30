using Jennifer.Infrastructure.Database;
using Jennifer.Infrastructure.Session.Abstracts;
using Jennifer.Infrastructure.Session.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Hybrid;

namespace Jennifer.Infrastructure.Session.Implements;

public sealed class OptionCacheProvider(
    HybridCache hybridCache,
    JenniferReadOnlyDbContext dbContext) : IOptionCacheProvider
{
    private OptionCacheResult[] _cached;
    public async Task<OptionCacheResult[]> GetAsync(string sid)
    {
        if (_cached is not null) return _cached;
        
        async ValueTask<OptionCacheResult[]> FetchFromDatabase(CancellationToken token) =>
            await dbContext.Options
                .Select(m => new OptionCacheResult
                {
                    Type = m.Type,
                    Value = m.Value
                })
                .ToArrayAsync(token);
        
        _cached = await hybridCache.GetOrCreateAsync(CachingConsts.OptionCacheKey(sid), FetchFromDatabase);
        
        return _cached;
    }

    public async Task ClearAsync(string sid)
    {
        var key = CachingConsts.OptionCacheKey(sid);
        await hybridCache.RemoveAsync(key);
    }
}