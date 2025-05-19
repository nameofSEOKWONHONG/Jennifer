using Jennifer.Tenant.Models;

namespace Jennifer.Tenant.Session.Abstracts;

public interface IUserRoleFetcher : ICachedFetcher<IEnumerable<UserRole>, Guid> { }
