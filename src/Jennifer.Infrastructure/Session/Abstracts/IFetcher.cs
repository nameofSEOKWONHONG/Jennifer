namespace Jennifer.Infrastructure.Session.Abstracts;

public interface IFetcher<TOut, in TIn>
{
    Task<TOut> FetchAsync(TIn sid);
}