using Jennifer.SharedKernel;

namespace Jennifer.Domain.Account;

public sealed record UserCompleteDomainEvent(User User): IDomainEvent;

