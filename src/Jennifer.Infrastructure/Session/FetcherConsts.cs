namespace Jennifer.Infrastructure.Session;

public sealed class CachingConsts
{
    /// <summary>
    /// SID의 값은 USERID이다.
    /// </summary>
    private const string SidCacheKeyFormat = "sid:{0}";
    public static string SidCacheKey(string sid) => string.Format(SidCacheKeyFormat, sid);
    
    private const string UserCacheKeyFormat = "user:{0}";
    public static string UserCacheKey(string userId) => string.Format(UserCacheKeyFormat, userId);
}