using eXtensionSharp;
using Jennifer.Account.Application.Auth.Contracts;
using Jennifer.Account.Application.Users.Filters;
using Jennifer.Account.Data;
using Jennifer.Account.Session.Abstracts;
using Jennifer.SharedKernel;
using LinqKit;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Account.Application.Users.Commands;

public class GetUserQueryHandler(ISessionContext context,
    IUserQueryFilter queryFilter): IQueryHandler<GetUserQuery, Result<UserDto>>
{
    public async ValueTask<Result<UserDto>> Handle(GetUserQuery query, CancellationToken cancellationToken)
    {
        var result = await context.ApplicationDbContext
            .xAs<JenniferDbContext>()
            .Users
            .AsNoTracking()
            .AsExpandable()
            .Where(queryFilter.Where(query))
            .Select(queryFilter.Selector)
            .FirstOrDefaultAsync(cancellationToken);
        
        return Result<UserDto>.Success(result);
    }
}