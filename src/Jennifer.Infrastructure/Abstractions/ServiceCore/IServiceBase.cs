using Jennifer.SharedKernel;

namespace Jennifer.Infrastructure.Abstractions.ServiceCore;

public interface IServiceBase<TRequest, TResult> where TResult : IResult
{
    ValueTask<TResult> ExecuteAsync(TRequest request, CancellationToken cancellationToken = default);
}