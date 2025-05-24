namespace Jennifer.Infrastructure.Abstractions.ServiceCore;

public interface IServiceBase<TRequest, TResult>
{
    Task<TResult> ExecuteAsync(TRequest request, CancellationToken cancellationToken = default);
}