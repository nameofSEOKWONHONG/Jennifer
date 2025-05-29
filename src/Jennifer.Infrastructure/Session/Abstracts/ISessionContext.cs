using Jennifer.Infrastructure.Session.Contracts;
using Jennifer.SharedKernel;

namespace Jennifer.Infrastructure.Session.Abstracts;

public interface ISessionContext
{
    IUnifiedContext<UserFetchResult> User { get; }
    IUnifiedContext<UserOptionFetchResult[]> UserOption { get; }
    IUnifiedContext<OptionFetchResult[]> Option { get; }
    IDomainEventPublisher DomainEventPublisher { get; }
    bool IsAuthenticated { get; }
}