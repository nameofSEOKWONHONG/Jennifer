using Jennifer.Infrastructure.Session.Contracts;
using Jennifer.SharedKernel;

namespace Jennifer.Infrastructure.Session;

public interface ISessionContext
{
    IUnifiedContext<UserCacheResult> User { get; }
    IUnifiedContext<UserOptionCacheResult[]> UserOption { get; }
    IUnifiedContext<OptionCacheResult[]> Option { get; }
    IDomainEventPublisher DomainEventPublisher { get; }
    bool IsAuthenticated { get; }
}