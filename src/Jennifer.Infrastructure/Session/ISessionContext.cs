using Jennifer.Infrastructure.Session.Contracts;
using Jennifer.SharedKernel;
using Mediator;

namespace Jennifer.Infrastructure.Session;

public interface ISessionContext
{
    IUnifiedContext<UserCacheResult> User { get; }
    IUnifiedContext<UserOptionCacheResult[]> UserOption { get; }
    IUnifiedContext<OptionCacheResult[]> Option { get; }
    IPublisher  DomainEventPublisher { get; }
    bool IsAuthenticated { get; }
}