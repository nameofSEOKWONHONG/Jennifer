namespace Jennifer.Infrastructure.Session;

public interface IUnifiedCacheProvider<TFetchResult>
{
    string Sid { get; }
    Task<TFetchResult> GetAsync();
    Task ClearAsync();
}