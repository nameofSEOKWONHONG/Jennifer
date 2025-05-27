using Jennifer.Account.Application.Auth.Contracts;
using Jennifer.SharedKernel;
using Mediator;

namespace Jennifer.Account.Application.Users.Commands;

public class GetsUserRequest : PagingRequest
{
    public string Email { get; set; }
    public string UserName { get; set; }
}

internal sealed record GetsUserQuery(string Email, string UserName, int PageNo = 1, int PageSize = 10)
    : IQuery<PaginatedResult<UserDto[]>>;