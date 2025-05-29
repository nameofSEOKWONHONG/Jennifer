namespace Jennifer.Infrastructure.Session.Abstracts;

public interface IUnifiedContext<TFetchResult>
{
    string Sid { get; }
    Task<TFetchResult> GetAsync();
    Task ClearAsync();
}