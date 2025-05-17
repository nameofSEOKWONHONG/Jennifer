namespace Jennifer.SharedKernel;

public interface IServiceBase<TRequest, TResponse>
{
    Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken);
}