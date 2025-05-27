namespace Jennifer.Account.Data;

internal sealed class CachingConsts
{
    private const string UserCacheKeyFormat = "user-{0}";
    public static string UserCacheKey(Guid userId) => string.Format(UserCacheKeyFormat, userId);
}