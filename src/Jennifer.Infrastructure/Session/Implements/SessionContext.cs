using Jennifer.Account.Session.Abstracts;
using Jennifer.Infrastructure.Session.Abstracts;
using Jennifer.SharedKernel;

namespace Jennifer.Infrastructure.Session.Implements;

public class SessionContext : ISessionContext
{
    public IUserContext User { get; }
    public IDomainEventPublisher DomainEventPublisher { get; }
    public bool IsAuthenticated => !string.IsNullOrEmpty(User?.Sid);

    public SessionContext(IUserContext user, IDomainEventPublisher domainEventPublisher)
    {
        User = user;
        DomainEventPublisher = domainEventPublisher;
    }
}