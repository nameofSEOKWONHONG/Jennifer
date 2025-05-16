using Microsoft.Extensions.Logging;

namespace Jennifer.Jwt.Services.Bases;

public abstract class ServiceBase<T, TRequest, TResponse>
{
    protected readonly ILogger<T> logger;

    protected ServiceBase(ILogger<T> logger)
    {
        logger = logger;
    }
}
