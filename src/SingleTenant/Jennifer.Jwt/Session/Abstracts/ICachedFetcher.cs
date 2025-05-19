using Jennifer.Jwt.Models;

namespace Jennifer.Jwt.Session.Abstracts;

public interface IUserRoleFetcher : ICachedFetcher<IEnumerable<UserRole>, Guid> { }
