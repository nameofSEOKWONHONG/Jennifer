namespace Jennifer.Jwt.Services.Bases;

public interface IServiceBase<TRequest, TResponse>
{
    Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken);
}