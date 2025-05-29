using System.Linq.Expressions;
using eXtensionSharp;
using Jennifer.Account.Application.Auth.Contracts;
using Jennifer.Account.Application.Users.Commands;
using Jennifer.Domain.Account;
using LinqKit;

namespace Jennifer.Account.Application.Users.Filters;

internal sealed class UserQueryFilter : IUserQueryFilter
{
    public Expression<Func<User, bool>> Where(GetsUserQuery query)
    {
        var predicate = PredicateBuilder.New<User>(true);
        predicate = predicate.And(x => x.IsDelete == false);
        if (query.Email.xIsNotEmpty())
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
    
    public Expression<Func<User, UserDto>> Selector { get; } = user => new UserDto
    {
        Id = user.Id,
        Email = user.Email,
        PhoneNumber = user.PhoneNumber,
        UserName = user.UserName,
        IsDelete = user.IsDelete,
        RoleNames = user.UserRoles.Select(x => x.Role.Name).ToArray(),
        UserClaims = user.UserClaims.Select(x => new UserClaimDto()
        {
            ClaimType = x.ClaimType,
            ClaimValue = x.ClaimValue,
        }).ToArray()
    };
    
    

    public Expression<Func<User, bool>> Where(RemoveUserCommand command)
    {
        var predicate = PredicateBuilder.New<User>(true);
        predicate = predicate.And(x => x.IsDelete == false);
        predicate = predicate.And(x => x.Id == command.UserId);
        return predicate;
    }
}