using Jennifer.Infrastructure.Session.Abstracts;
using Jennifer.SharedKernel;

namespace Jennifer.Account.Session.Abstracts;

public interface ISessionContext
{
    IUserContext User { get; }
    IDomainEventPublisher DomainEventPublisher { get; }
    bool IsAuthenticated { get; }
}