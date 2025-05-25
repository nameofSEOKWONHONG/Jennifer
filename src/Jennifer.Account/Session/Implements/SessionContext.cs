using eXtensionSharp;
using Jennifer.Account.Data;
using Jennifer.Account.Session.Abstracts;
using Jennifer.Infrastructure.Abstractions.Behaviors;
using Jennifer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

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