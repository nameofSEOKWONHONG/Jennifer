using Jennifer.Infrastructure.Session.Contracts;

namespace Jennifer.Infrastructure.Session.Abstracts;

public interface IUserCacheProvider : ICacheProvider<UserCacheResult, string>;