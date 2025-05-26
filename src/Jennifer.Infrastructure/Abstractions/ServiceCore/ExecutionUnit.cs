using eXtensionSharp;
using Jennifer.SharedKernel;
using Microsoft.Extensions.DependencyInjection;

namespace Jennifer.Infrastructure.Abstractions.ServiceCore;

public class ExecutionUnit<TService, TRequest, TResult> : IExecutionUnit, IServiceRegistration<TService, TRequest, TResult>
    where TService : class, IServiceBase<TRequest, TResult>
    where TResult : IResult, new()
{
    private readonly IServiceProvider _provider;
    private readonly ServiceExecutionBuilder _builder;
    private Func<bool> _condition = () => true;
    private TRequest _request;
    private Action<TResult> _resultHandler;

    public ExecutionUnit(IServiceProvider provider, ServiceExecutionBuilder builder)
    {
        _provider = provider;
        _builder = builder;
    }

    public IServiceRegistration<TService, TRequest, TResult> When(Func<bool> predicate)
    {
        _condition = predicate ?? (() => true);
        return this;
    }

    public IServiceRegistration<TService, TRequest, TResult> Request(TRequest request)
    {
        _request = request;
        return this;
    }

    public ServiceExecutionBuilder Handle(Action<TResult> handler)
    {
        _resultHandler = handler;
        return _builder;
    }

    public bool CanExecute() => _condition();

    public async Task<object> ExecuteAsync(CancellationToken cancellationToken)
    {
        if (_request == null)
            throw new InvalidOperationException("Request is not set.");

        var service = _provider.GetRequiredService<TService>();

        if (service is ServiceBase<TRequest, TResult> baseService)
            baseService.SetServiceProvider(_provider);

        var result = await service.ExecuteAsync(_request, cancellationToken);
        return result;
    }

    public void ApplyResult(object result)
    {
        if (_resultHandler.xIsEmpty())
        {
            result = new TResult() { IsSuccess = false, Message = "Result handler is not set."};
        }
        else if (result.xIsEmpty())
        {
            result = new TResult() { IsSuccess = false, Message = "Previous when is not passed or current when is not passed."};
        }
        _resultHandler((TResult)result);
    }
}