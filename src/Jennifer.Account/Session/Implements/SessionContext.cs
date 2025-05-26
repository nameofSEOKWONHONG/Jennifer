using Jennifer.Account.Session.Abstracts;
using Jennifer.Infrastructure.Abstractions.Behaviors;

namespace Jennifer.Account.Session.Implements;

public class SessionContext : ISessionContext
{
    public IUserContext User { get; }
    public IDomainEventPublisher DomainEventPublisher { get; }
    public bool IsAuthenticated => !string.IsNullOrEmpty(User?.UserId);

    public SessionContext(IUserContext user, IDomainEventPublisher domainEventPublisher)
    {
        User = user;
        DomainEventPublisher = domainEventPublisher;
    }
}