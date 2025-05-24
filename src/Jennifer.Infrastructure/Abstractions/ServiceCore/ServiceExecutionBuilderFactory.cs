namespace Jennifer.Infrastructure.Abstractions.ServiceCore;

public class ServiceExecutionBuilderFactory : IServiceExecutionBuilderFactory
{
    private readonly IServiceProvider _provider;
    public ServiceExecutionBuilderFactory(IServiceProvider provider)
    {
        _provider = provider;
    }

    public ServiceExecutionBuilder Create()
        => new ServiceExecutionBuilder(_provider);
}