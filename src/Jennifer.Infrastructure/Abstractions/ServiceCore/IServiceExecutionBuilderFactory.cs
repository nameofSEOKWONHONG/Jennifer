using Microsoft.Extensions.DependencyInjection;

namespace Jennifer.Infrastructure.Abstractions.ServiceCore;

public interface IServiceExecutionBuilderFactory
{
    ServiceExecutionBuilder Create();
}

