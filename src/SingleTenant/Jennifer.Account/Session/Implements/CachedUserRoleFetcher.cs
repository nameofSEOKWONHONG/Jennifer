using Jennifer.Account.Models;
using Jennifer.Account.Session.Abstracts;

namespace Jennifer.Account.Session.Implements;

/// <summary>
/// Provides a caching mechanism for fetching user roles to optimize data retrieval by reducing redundant calls.
/// </summary>
/// <remarks>
/// This class decorates another implementation of <see cref="IUserRoleFetcher"/> to support caching.
/// Once data is fetched for the first time, subsequent calls return the cached result until the application lifecycle ends or the cache is explicitly refreshed.
/// </remarks>
public class CachedUserRoleFetcher : IUserRoleFetcher
{
    private readonly IUserRoleFetcher _inner;
    private IEnumerable<UserRole> _cached;
    private bool _isCached = false;

    public CachedUserRoleFetcher(IUserRoleFetcher inner)
    {
        _inner = inner;
    }

    public async Task<IEnumerable<UserRole>> FetchAsync(Guid input)
    {
        if (_isCached) return _cached;

        _cached = await _inner.FetchAsync(input);
        _isCached = true;
        return _cached;
    }
}