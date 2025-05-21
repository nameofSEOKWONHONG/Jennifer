namespace Jennifer.Jwt.Session.Abstracts;

public interface ICachedFetcher<TOut, in TIn>
{
    Task<TOut> HandleAsync(TIn input);
}