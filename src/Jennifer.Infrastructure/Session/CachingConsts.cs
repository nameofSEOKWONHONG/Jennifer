namespace Jennifer.Infrastructure.Session;

public sealed class CachingConsts
{
    /// <summary>
    /// SID의 값은 USERID이다.
    /// </summary>
    private const string SidCacheKeyFormat = "sid:{0}";
    public static string SidCacheKey(string sid) => string.Format(SidCacheKeyFormat, sid);
    
    private const string UserCacheKeyFormat = "sid:{0}:user:{1}";
    public static string UserCacheKey(string sid, string userId) => string.Format(UserCacheKeyFormat, sid, userId);
    
    private const string OptionCacheKeyFormat = "option:{0}";
    public static string OptionCacheKey(string sid) => string.Format(OptionCacheKeyFormat, sid);
    
    private const string UserOptionCacheKeyFormat = "useroption:{0}";
    public static string UserOptionCacheKey(string sid) => string.Format(UserOptionCacheKeyFormat, sid);
}