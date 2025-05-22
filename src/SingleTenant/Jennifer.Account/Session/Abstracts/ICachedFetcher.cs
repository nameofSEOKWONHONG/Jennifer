using Jennifer.Account.Models;

namespace Jennifer.Account.Session.Abstracts;

public interface IUserRoleFetcher : ICachedFetcher<IEnumerable<UserRole>, Guid> { }
