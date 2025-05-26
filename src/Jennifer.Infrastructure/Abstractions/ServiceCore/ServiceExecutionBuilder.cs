using Jennifer.SharedKernel;

namespace Jennifer.Infrastructure.Abstractions.ServiceCore;

public class ServiceExecutionBuilder
{
    private readonly IServiceProvider _provider;
    private readonly List<IExecutionUnit> _units = new();

    public ServiceExecutionBuilder(IServiceProvider provider)
    {
        _provider = provider;
    }

    public IServiceRegistration<TService, TRequest, TResult> Register<TService, TRequest, TResult>()
        where TService : class, IServiceBase<TRequest, TResult>
        where TResult : IResult, new()
    {
        var unit = new ExecutionUnit<TService, TRequest, TResult>(_provider, this);
        _units.Add(unit);
        return unit;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        foreach (var unit in _units)
        {
            object result = null;

            if (unit.CanExecute())
            {
                result = await unit.ExecuteAsync(cancellationToken);
            }

            unit.ApplyResult(result);
        }
    }
}