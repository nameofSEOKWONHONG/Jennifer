using eXtensionSharp;
using Jennifer.Infrastructure.Session.Abstracts;
using Jennifer.Infrastructure.Session.Contracts;
using Microsoft.AspNetCore.Http;

namespace Jennifer.Infrastructure.Session;


public sealed class UserCacheProvider(IHttpContextAccessor httpContextAccessor,
    IUserCacheProvider userCacheProvider) : CacheProviderBase<UserCacheResult>(httpContextAccessor)
{
    public override async Task<UserCacheResult> GetAsync()
    {
        if(this.Sid.xIsEmpty()) return null;
        return await userCacheProvider.GetAsync(this.Sid);
    }

    public override async Task ClearAsync()
    {
        if(this.Sid.xIsEmpty()) throw new Exception("Sid is empty");
        await userCacheProvider.ClearAsync(this.Sid);
    }
        
}

public sealed class OptionCacheProvider(
    IHttpContextAccessor httpContextAccessor,
    IOptionCacheProvider optionProvider) : CacheProviderBase<OptionCacheResult[]>(httpContextAccessor)
{
    public override async Task<OptionCacheResult[]> GetAsync()
    {
        if(this.Sid.xIsEmpty()) return null;
        return await optionProvider.GetAsync(this.Sid);   
    }

    public override async Task ClearAsync()
    {
        if(this.Sid.xIsEmpty()) throw new Exception("Sid is empty");
        await optionProvider.ClearAsync(this.Sid);
    }
}

public sealed class UserOptionCacheProvider(
    IHttpContextAccessor httpContextAccessor,
    IUserOptionCacheProvider userOptionCacheProvider) : CacheProviderBase<UserOptionCacheResult[]>(httpContextAccessor)
{
    public override async Task<UserOptionCacheResult[]> GetAsync()
    {
        if(this.Sid.xIsEmpty()) return null;
        return await userOptionCacheProvider.GetAsync(this.Sid);
    }

    public override async Task ClearAsync()
    {
        if(this.Sid.xIsEmpty()) throw new Exception("Sid is empty");
        await userOptionCacheProvider.ClearAsync(this.Sid);
    }
}