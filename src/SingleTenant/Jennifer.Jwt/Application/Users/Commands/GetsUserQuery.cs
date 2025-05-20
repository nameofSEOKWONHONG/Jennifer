using Jennifer.Infrastructure.Abstractions.Messaging;
using Jennifer.Jwt.Application.Auth.Contracts;
using Jennifer.SharedKernel;

namespace Jennifer.Jwt.Application.Users.Commands;

public class GetsUserRequest : PagingRequest
{
    public string Email { get; set; }
    public string UserName { get; set; }
}

public sealed record GetsUserQuery(string Email, string UserName, int PageNo = 1, int PageSize = 10)
    : IQuery<PagingResult<UserDto>>;