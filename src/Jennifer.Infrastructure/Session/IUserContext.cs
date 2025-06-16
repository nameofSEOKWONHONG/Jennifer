using System.Security.Claims;
using eXtensionSharp;
using eXtensionSharp.AspNet;
using Microsoft.AspNetCore.Http;

namespace Jennifer.Infrastructure.Session;

public interface IUnifiedCacheProvider<TFetchResult>
{
    string Sid { get; }
    Task<TFetchResult> GetAsync();
    Task ClearAsync();
}

public abstract class CacheProviderBase<TFetchResult>(IHttpContextAccessor httpContextAccessor) : IUnifiedCacheProvider<TFetchResult>
{
    public string Sid => httpContextAccessor.HttpContext.xGetClaim<string>(ClaimTypes.NameIdentifier);

    public abstract Task<TFetchResult> GetAsync();

    public abstract Task ClearAsync();
}