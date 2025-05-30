using Jennifer.SharedKernel;

namespace Jennifer.Domain.Accounts;

public sealed record UserCompleteDomainEvent(User User): IDomainEvent;

