using System.Linq.Expressions;
using Jennifer.Account.Application.Auth.Contracts;
using Jennifer.Account.Application.Users.Commands;
using Jennifer.Domain.Account;

namespace Jennifer.Account.Application.Users.Filters;

internal interface IUserQueryFilter
{
    Expression<Func<User, bool>> Where(GetsUserQuery query);
    Expression<Func<User, bool>> Where(GetUserQuery query);
    Expression<Func<User, bool>> Where(RemoveUserCommand command);
    Expression<Func<User, UserDto>> Selector { get; }
}