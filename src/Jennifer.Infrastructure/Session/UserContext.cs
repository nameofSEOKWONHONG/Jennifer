using System.Security.Claims;
using eXtensionSharp.AspNet;
using Jennifer.Infrastructure.Session.Abstracts;
using Jennifer.Infrastructure.Session.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Jennifer.Infrastructure.Session;


public sealed class UserCacheProvider(IHttpContextAccessor httpContextAccessor,
    IUserCacheProvider userCacheProvider) : IUnifiedCacheProvider<UserCacheResult>
{
    public string Sid => httpContextAccessor.HttpContext.xGetClaim<string>(ClaimTypes.NameIdentifier);
    public async Task<UserCacheResult> GetAsync()
        => await userCacheProvider.GetAsync(this.Sid);

    public async Task ClearAsync()
        => await userCacheProvider.ClearAsync(this.Sid);
}

public sealed class OptionCacheProvider(
    IHttpContextAccessor httpContextAccessor,
    IOptionCacheProvider optionProvider) : IUnifiedCacheProvider<OptionCacheResult[]>
{
    public string Sid => httpContextAccessor.HttpContext.xGetClaim<string>(ClaimTypes.NameIdentifier);
    public async Task<OptionCacheResult[]> GetAsync() => await optionProvider.GetAsync(this.Sid);

    public async Task ClearAsync() => await optionProvider.ClearAsync(this.Sid);
}

public sealed class UserOptionCacheProvider(
    IHttpContextAccessor httpContextAccessor,
    IUserOptionCacheProvider userOptionCacheProvider) : IUnifiedCacheProvider<UserOptionCacheResult[]>
{
    public string Sid => httpContextAccessor.HttpContext.xGetClaim<string>(ClaimTypes.NameIdentifier);
    public async Task<UserOptionCacheResult[]> GetAsync()
        => await userOptionCacheProvider.GetAsync(this.Sid);

    public async Task ClearAsync()
        => await userOptionCacheProvider.ClearAsync(this.Sid);
}