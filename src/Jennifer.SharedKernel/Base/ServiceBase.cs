using Microsoft.Extensions.Logging;

namespace Jennifer.SharedKernel.Base;

public abstract class ServiceBase<T, TRequest, TResponse>
{
    protected readonly ILogger<T> logger;

    protected ServiceBase(ILogger<T> logger)
    {
        logger = logger;
    }
}
