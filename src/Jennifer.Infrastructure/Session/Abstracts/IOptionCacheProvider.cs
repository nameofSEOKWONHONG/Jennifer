using Jennifer.Infrastructure.Session.Contracts;

namespace Jennifer.Infrastructure.Session.Abstracts;

public interface IOptionCacheProvider : ICacheProvider<OptionCacheResult[], string>;