using Jennifer.Infrastructure.Abstractions.Behaviors;
using Jennifer.Infrastructure.Data;

namespace Jennifer.Account.Session.Abstracts;

public interface ISessionContext
{
    IUserContext User { get; }
    IDomainEventPublisher DomainEventPublisher { get; }
    bool IsAuthenticated { get; }
}