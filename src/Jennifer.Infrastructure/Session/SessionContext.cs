using System.Security.Claims;
using Jennifer.Infrastructure.Session.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Jennifer.Infrastructure.Session;

public static class SessionContextKeyedServiceName
{
    public const string User = "User";
    public const string Option = "Option";
    public const string UserOption = "UserOption";
}

public class SessionContext : ISessionContext
{
    public string Sid { get; }
    public IUserContext User { get; }
    public IUnifiedCacheProvider<OptionCacheResult[]> Option { get; }
    public bool IsAuthenticated => !string.IsNullOrEmpty(this.Sid);

    public SessionContext(
        IUserContext userContext,
        IHttpContextAccessor httpContextAccessor,
        [FromKeyedServices(SessionContextKeyedServiceName.Option)] IUnifiedCacheProvider<OptionCacheResult[]> option)
    {
        Sid = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        User = userContext;
        Option = option;
    }
}

/// <summary>
/// User 의 상태를 조회하기 위한 Context
/// </summary>
public interface IUserContext
{
    /// <summary>
    /// 현재 상태
    /// </summary>
    IUnifiedCacheProvider<UserCacheResult> Current { get; }
    /// <summary>
    /// 유저의 옵션 상태
    /// </summary>
    IUnifiedCacheProvider<UserOptionCacheResult[]> Option { get; }
    
    /// <summary>
    /// 전체 초기화. 데이터가 변경된다면 호출되어야 함.
    /// </summary>
    /// <returns></returns>
    Task ClearAsync();
}

/// <summary>
/// 유저의 상태 조회. (왜 UserContext가 독립되었는가? ClearAsync와 하나로 다루기 위해.)
/// </summary>
/// <param name="user"></param>
/// <param name="userOption"></param>
public class UserContext(
    [FromKeyedServices(SessionContextKeyedServiceName.User)] IUnifiedCacheProvider<UserCacheResult> user,
    [FromKeyedServices(SessionContextKeyedServiceName.UserOption)] IUnifiedCacheProvider<UserOptionCacheResult[]> userOption) : IUserContext
{
    public IUnifiedCacheProvider<UserCacheResult> Current => user;
    public IUnifiedCacheProvider<UserOptionCacheResult[]> Option => userOption;

    public async Task ClearAsync()
    {
        await user.ClearAsync();
        await userOption.ClearAsync();   
    }
}