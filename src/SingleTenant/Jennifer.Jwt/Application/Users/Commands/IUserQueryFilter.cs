using System.Linq.Expressions;
using Jennifer.Jwt.Application.Auth.Contracts;
using Jennifer.Jwt.Models;

namespace Jennifer.Jwt.Application.Users.Commands;

public interface IUserQueryFilter
{
    Expression<Func<User, bool>> Where(GetsUserQuery query);
    Expression<Func<User, bool>> Where(GetUserQuery query);
    Expression<Func<User, bool>> Where(RemoveUserCommand command);
    Expression<Func<User, UserDto>> Selector { get; }
}