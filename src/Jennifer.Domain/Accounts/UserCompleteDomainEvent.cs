using Mediator;

namespace Jennifer.Domain.Accounts;

public sealed record UserCompleteDomainEvent(User User): INotification;

public sealed record SignUpConfirmedDomainEvent(User User): INotification;