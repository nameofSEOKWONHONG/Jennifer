using Jennifer.Domain.Accounts;
using Mediator;

namespace Jennifer.Account.Application.Auth.Commands.SignUp;

public sealed record UserCompleteDomainEvent(User User): INotification;

