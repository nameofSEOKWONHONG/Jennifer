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

internal sealed class GetUserQueryHandler(
    IUserQueryFilter queryFilter,
    JenniferDbContext dbContext): IQueryHandler<GetUserQuery, Result<UserDto>>
{
    public async ValueTask<Result<UserDto>> Handle(GetUserQuery query, CancellationToken cancellationToken)
    {
        var result = await dbContext
            .Users
            .Include(m => m.UserClaims)
            .AsNoTracking()
            .AsExpandable()
            .Where(queryFilter.Where(query))
            .Select(queryFilter.Selector)
            .FirstOrDefaultAsync(cancellationToken);
        
        return Result<UserDto>.Success(result);
    }
}