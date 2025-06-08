using Jennifer.Domain.Accounts;
using Mediator;

namespace Jennifer.Account.Application.Users.Commands;

public sealed class UserWithdrawDomainEventHandler() : INotificationHandler<UserWithdrawDomainEvent>
{
    public ValueTask Handle(UserWithdrawDomainEvent notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}