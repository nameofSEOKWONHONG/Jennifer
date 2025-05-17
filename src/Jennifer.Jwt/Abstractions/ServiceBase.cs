using eXtensionSharp;
using Jennifer.Jwt.Infrastructure.Session;
using Microsoft.Extensions.Logging;

namespace Jennifer.Jwt.Abstractions;

public abstract class ServiceBase<T, TRequest, TResponse>
{
    protected readonly ILogger<T> logger;
    protected ServiceBase(ILogger<T> logger)
    {
        this.logger = logger;
    }
}

public abstract class SessionServiceBase<T, TRequest, TResponse> : ServiceBase<T, TRequest, TResponse>
{
    private readonly ISessionContext _sessionContext;

    protected SessionServiceBase(ILogger<T> logger, ISessionContext sessionContext): base(logger)
    {
        _sessionContext = sessionContext;
    }

    protected TDb AsDatabase<TDb>() => _sessionContext.ApplicationDbContext.xAs<TDb>();
}
