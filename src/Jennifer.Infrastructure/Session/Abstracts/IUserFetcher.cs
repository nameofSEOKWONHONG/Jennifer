using Jennifer.Account.Session.Abstracts;
using Jennifer.Domain.Account;

namespace Jennifer.Infrastructure.Session.Abstracts;

// internal interface IUserRoleFetcher : ICachedFetcher<IEnumerable<UserRole>, Guid> { }

public interface IUserFetcher : IFetcher<User, string>;
public interface IUserRoleFetcher : IFetcher<IEnumerable<UserRole>, Guid>;