using Jennifer.Domain.Accounts.Contracts;

namespace Jennifer.Infrastructure.Session.Contracts;

public sealed record UserOptionCacheResult(ENUM_USER_OPTION_TYPE Type, string Value);