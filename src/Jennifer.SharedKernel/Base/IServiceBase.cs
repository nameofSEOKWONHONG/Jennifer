namespace Jennifer.SharedKernel.Base;

public interface IServiceBase<TRequest, TResponse>
{
    Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken);
}