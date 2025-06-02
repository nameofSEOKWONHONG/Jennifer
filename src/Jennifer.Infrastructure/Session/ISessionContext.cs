using Jennifer.Infrastructure.Abstractions.Behaviors;
using Jennifer.Infrastructure.Session.Contracts;
using Jennifer.SharedKernel;
using Mediator;

namespace Jennifer.Infrastructure.Session;

public interface ISessionContext
{
    IUserContext User { get; }
    IUnifiedCacheProvider<OptionCacheResult[]> Option { get; }
    bool IsAuthenticated { get; }
}