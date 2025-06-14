﻿using System.Linq.Expressions;
using Jennifer.Account.Application.Users.Commands;
using Jennifer.Domain.Accounts;
using Jennifer.SharedKernel.Account.Auth;

namespace Jennifer.Account.Application.Users.Queries;

public interface IUserQueryFilter
{
    Expression<Func<User, bool>> Where(GetsUserQuery query);
    Expression<Func<User, bool>> Where(GetUserQuery query);
    Expression<Func<User, bool>> Where(RemoveUserCommand command);
    Expression<Func<User, UserDto>> Selector { get; }
}