using System.Security.Claims;
using eXtensionSharp.AspNet;
using Jennifer.Infrastructure.Session.Abstracts;
using Jennifer.Infrastructure.Session.Contracts;
using Microsoft.AspNetCore.Http;

namespace Jennifer.Infrastructure.Session;

public sealed class UserContext(IHttpContextAccessor httpContextAccessor,
    IUserCacheProvider userCacheProvider) : IUnifiedContext<UserCacheResult>
{
    public string Sid => httpContextAccessor.HttpContext.xGetClaim<string>(ClaimTypes.NameIdentifier);
    public async Task<UserCacheResult> GetAsync()
        => await userCacheProvider.GetAsync(this.Sid);

    public async Task ClearAsync()
        => await userCacheProvider.ClearAsync(this.Sid);
}

public sealed class OptionContext(
    IHttpContextAccessor httpContextAccessor,
    IOptionCacheProvider optionProvider) : IUnifiedContext<OptionCacheResult[]>
{
    public string Sid => httpContextAccessor.HttpContext.xGetClaim<string>(ClaimTypes.NameIdentifier);
    public async Task<OptionCacheResult[]> GetAsync() => await optionProvider.GetAsync(this.Sid);

    public async Task ClearAsync() => await optionProvider.ClearAsync(this.Sid);
}

public sealed class UserOptionContext(
    IHttpContextAccessor httpContextAccessor,
    IUserOptionCacheProvider userOptionCacheProvider) : IUnifiedContext<UserOptionCacheResult[]>
{
    public string Sid => httpContextAccessor.HttpContext.xGetClaim<string>(ClaimTypes.NameIdentifier);
    public async Task<UserOptionCacheResult[]> GetAsync()
        => await userOptionCacheProvider.GetAsync(this.Sid);

    public async Task ClearAsync()
        => await userOptionCacheProvider.ClearAsync(this.Sid);
}