namespace Jennifer.Account.Session.Abstracts;

public interface ICachedFetcher<TOut, in TIn>
{
    Task<TOut> FetchAsync(TIn input);
}