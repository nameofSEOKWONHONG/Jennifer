﻿using Jennifer.Account.Application.Auth.Contracts;
using Jennifer.SharedKernel;
using Mediator;

namespace Jennifer.Account.Application.Users.Queries;

public class GetsUserRequest : PagingRequest
{
    public string Email { get; set; }
    public string UserName { get; set; }
}

public sealed record GetsUserQuery(string Email, string UserName, int PageNo = 1, int PageSize = 10)
    : IQuery<PaginatedResult<UserDto>>;