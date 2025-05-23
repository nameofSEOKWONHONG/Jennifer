using Jennifer.Account.Models;

namespace Jennifer.Account.Session.Abstracts;

// internal interface IUserRoleFetcher : ICachedFetcher<IEnumerable<UserRole>, Guid> { }
public interface IUserFetcher : IFetcher<User, Guid>;
public interface IUserRoleFetcher : IFetcher<IEnumerable<UserRole>, Guid>;