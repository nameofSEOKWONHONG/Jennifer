using Jennifer.Jwt.Models;

namespace Jennifer.Jwt.Infrastructure.Session.Abstracts;

public interface IUserRoleFetcher : ICachedFetcher<IEnumerable<UserRole>, Guid> { }
