namespace Jennifer.Infrastructure.Abstractions.ServiceCore;

public interface IServiceRegistration<TService, TRequest, TResult>
{
    IServiceRegistration<TService, TRequest, TResult> Where(Func<bool> predicate);
    IServiceRegistration<TService, TRequest, TResult> Request(TRequest request);
    ServiceExecutionBuilder Handle(Action<TResult> handler);
}