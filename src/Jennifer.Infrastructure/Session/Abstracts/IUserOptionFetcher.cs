using Jennifer.Domain.Account.Contracts;
using Jennifer.Infrastructure.Session.Contracts;

namespace Jennifer.Infrastructure.Session.Abstracts;

public sealed record UserOptionFetchResult(ENUM_USER_OPTION_TYPE Type, string Value);
public interface IUserOptionFetcher: IFetcher<UserOptionFetchResult[], string>;