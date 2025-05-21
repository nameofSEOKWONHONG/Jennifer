using eXtensionSharp;
using Jennifer.Jwt.Application.Auth.Contracts;
using Jennifer.Jwt.Data;
using Jennifer.Jwt.Session.Abstracts;
using Jennifer.SharedKernel;
using LinqKit;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Jwt.Application.Users.Commands;

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