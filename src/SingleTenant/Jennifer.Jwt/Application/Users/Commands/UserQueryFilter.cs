using System.Linq.Expressions;
using eXtensionSharp;
using Jennifer.Jwt.Application.Auth.Contracts;
using Jennifer.Jwt.Models;
using LinqKit;

namespace Jennifer.Jwt.Application.Users.Commands;

public class UserQueryFilter : IUserQueryFilter
{
    public Expression<Func<User, bool>> Where(GetsUserQuery query)
    {
        var predicate = PredicateBuilder.New<User>(true);
        predicate = predicate.And(x => x.IsDelete == false);
        if (query.xIsNotEmpty())
        {
            predicate = predicate.And(x => x.Email.Contains(query.Email));
        }
        
        if(query.UserName.xIsNotEmpty())
        {
            predicate = predicate.And(x => x.UserName == query.UserName);
        }
        
        return predicate;
    }

    public Expression<Func<User, bool>> Where(GetUserQuery query)
    {
        var predicate = PredicateBuilder.New<User>(true);
        predicate = predicate.And(x => x.IsDelete == false);
        predicate = predicate.And(x => x.Id == query.UserId);
        return predicate;
    }

    public Expression<Func<User, bool>> Where(RemoveUserCommand command)
    {
        var predicate = PredicateBuilder.New<User>(true);
        predicate = predicate.And(x => x.IsDelete == false);
        predicate = predicate.And(x => x.Id == command.UserId);
        return predicate;
    }

    public Expression<Func<User, UserDto>> Selector { get; } = user => new UserDto
    {
        Id = user.Id,
        Email = user.Email,
        PhoneNumber = user.PhoneNumber,
        UserName = user.UserName,
        IsDelete = user.IsDelete
    };
}