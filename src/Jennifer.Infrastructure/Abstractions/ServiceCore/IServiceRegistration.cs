using Jennifer.SharedKernel;

namespace Jennifer.Infrastructure.Abstractions.ServiceCore;

public interface IServiceRegistration<TService, TRequest, TResult> where TResult : IResult
{
    IServiceRegistration<TService, TRequest, TResult> When(Func<bool> predicate);
    IServiceRegistration<TService, TRequest, TResult> Request(TRequest request);
    ServiceExecutionBuilder Handle(Action<TResult> handler);
}