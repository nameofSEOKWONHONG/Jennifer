using Mediator;

namespace Jennifer.Domain.Accounts;

public sealed record UserWithdrawDomainEvent(User user) : INotification;