using Jennifer.Infrastructure.Abstractions.Behaviors;

namespace Jennifer.Account.Session.Abstracts;

public interface ISessionContext
{
    IUserContext User { get; }
    IDomainEventPublisher DomainEventPublisher { get; }
    bool IsAuthenticated { get; }
}