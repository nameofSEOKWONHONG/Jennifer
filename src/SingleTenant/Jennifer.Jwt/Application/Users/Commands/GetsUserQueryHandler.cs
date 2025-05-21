using eXtensionSharp;
using Jennifer.Infrastructure.Abstractions.Behaviors;
using Jennifer.Infrastructure.Abstractions.Messaging;
using Jennifer.Jwt.Application.Auth.Contracts;
using Jennifer.Jwt.Data;
using Jennifer.Jwt.Session.Abstracts;
using Jennifer.SharedKernel;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Jwt.Application.Users.Commands;

public class GetsUserQueryHandler(ISessionContext context,
    JenniferDbContext dbContext,
    IUserQueryFilter queryFilter,
    ISender sender) : IQueryHandler<GetsUserQuery, PagingResult<UserDto>>
{
    public async Task<Result<PagingResult<UserDto>>> HandleAsync(GetsUserQuery query, CancellationToken cancellationToken)
    {
        // var roles1 = await context.UserContext.GetUserRolesAsync();
        // var roles2 = await context.UserContext.GetUserRolesAsync();
        var queryable = dbContext
            .Users.AsNoTracking()
            .AsExpandable()
            .Where(queryFilter.Where(query));
        
        var total = await queryable
            .CountAsync(cancellationToken);
        var result = await queryable
            .Skip((query.PageNo - 1) * query.PageSize)
            .Take(query.PageSize)       
            .Select(queryFilter.Selector)
            .ToArrayAsync(cancellationToken: cancellationToken);

        var result1 = await sender.Send(new GetUserQuery(result[0].Id), cancellationToken);
        var result3 = await sender.Send(new ModifyUserCommand(result[0]), cancellationToken);
        var result2 = await sender.Send(new RemoveUserCommand(result[0].Id), cancellationToken);
        
        return PagingResult<UserDto>.Create(total, result, query.PageNo, query.PageSize);
    }
}