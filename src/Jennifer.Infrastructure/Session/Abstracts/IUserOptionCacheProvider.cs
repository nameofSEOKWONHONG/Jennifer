using Jennifer.Domain.Accounts.Contracts;
using Jennifer.Infrastructure.Session.Contracts;

namespace Jennifer.Infrastructure.Session.Abstracts;

public interface IUserOptionCacheProvider: ICacheProvider<UserOptionCacheResult[], string>;