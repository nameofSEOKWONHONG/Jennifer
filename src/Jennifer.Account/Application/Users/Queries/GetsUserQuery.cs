using Jennifer.SharedKernel;
using Jennifer.SharedKernel.Account.Auth;
using Mediator;

namespace Jennifer.Account.Application.Users.Queries;



public sealed record GetsUserQuery(string Email, string UserName, int PageNo = 1, int PageSize = 10)
    : IQuery<PaginatedResult<UserDto>>;