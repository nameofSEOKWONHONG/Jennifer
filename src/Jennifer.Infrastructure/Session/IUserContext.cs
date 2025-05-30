namespace Jennifer.Infrastructure.Session;

public interface IUnifiedContext<TFetchResult>
{
    string Sid { get; }
    Task<TFetchResult> GetAsync();
    Task ClearAsync();
}