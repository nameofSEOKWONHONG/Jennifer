using Jennifer.Domain.Accounts;
using Jennifer.Domain.Accounts.Contracts;
using Jennifer.SharedKernel;
using Mediator;

namespace Jennifer.Account.Application.Options.Queries;

public sealed record GetsOptionQuery(ENUM_OPTION_TYPE Type, int PageNo, int PageSize): IQuery<PaginatedResult<Option>>;
