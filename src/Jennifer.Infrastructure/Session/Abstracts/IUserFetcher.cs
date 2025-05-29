using Jennifer.Infrastructure.Session.Contracts;

namespace Jennifer.Infrastructure.Session.Abstracts;

// internal interface IUserRoleFetcher : ICachedFetcher<IEnumerable<UserRole>, Guid> { }

public interface IUserFetcher : IFetcher<UserFetchResult, string>;

public interface IOptionFetcher : IFetcher<OptionFetchResult[], string>;