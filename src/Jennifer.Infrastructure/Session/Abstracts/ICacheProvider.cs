namespace Jennifer.Infrastructure.Session.Abstracts;

public interface ICacheProvider<TOut, in TIn>
{
    Task<TOut> GetAsync(TIn sid);
    Task ClearAsync(TIn sid);
}