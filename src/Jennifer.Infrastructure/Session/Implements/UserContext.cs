using System.Security.Claims;
using eXtensionSharp.AspNet;
using Jennifer.Infrastructure.Session.Abstracts;
using Jennifer.Infrastructure.Session.Contracts;
using Microsoft.AspNetCore.Http;

namespace Jennifer.Infrastructure.Session.Implements;

public sealed class UserContext(IHttpContextAccessor httpContextAccessor,
    IUserFetcher userFetcher) : IUnifiedContext<UserFetchResult>
{
    public string Sid => httpContextAccessor.HttpContext.xGetClaim<string>(ClaimTypes.NameIdentifier);
    public async Task<UserFetchResult> GetAsync()
        => await userFetcher.FetchAsync(this.Sid);

    public async Task ClearAsync()
        => await userFetcher.ClearAsync(this.Sid);
}

public sealed class OptionContext(
    IHttpContextAccessor httpContextAccessor,
    IOptionFetcher optionFetcher) : IUnifiedContext<OptionFetchResult[]>
{
    public string Sid => httpContextAccessor.HttpContext.xGetClaim<string>(ClaimTypes.NameIdentifier);
    public async Task<OptionFetchResult[]> GetAsync() => await optionFetcher.FetchAsync(this.Sid);

    public async Task ClearAsync() => await optionFetcher.ClearAsync(this.Sid);
}

public sealed class UserOptionContext(
    IHttpContextAccessor httpContextAccessor,
    IUserOptionFetcher userOptionFetcher) : IUnifiedContext<UserOptionFetchResult[]>
{
    public string Sid => httpContextAccessor.HttpContext.xGetClaim<string>(ClaimTypes.NameIdentifier);
    public async Task<UserOptionFetchResult[]> GetAsync()
        => await userOptionFetcher.FetchAsync(this.Sid);

    public async Task ClearAsync()
        => await userOptionFetcher.ClearAsync(this.Sid);
}