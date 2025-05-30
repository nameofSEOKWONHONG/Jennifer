using Jennifer.Account.Application.Auth.Contracts;
using Jennifer.Account.Application.Users.Filters;
using Jennifer.Infrastructure.Database;
using Jennifer.Infrastructure.Session.Abstracts;
using Jennifer.SharedKernel;
using LinqKit;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Account.Application.Users.Commands;

public sealed class GetsUserQueryHandler(
    ISessionContext session,
    IUserQueryFilter queryFilter,
    JenniferDbContext dbContext) : IQueryHandler<GetsUserQuery, PaginatedResult<UserDto>>
{
    public async ValueTask<PaginatedResult<UserDto>> Handle(GetsUserQuery query, CancellationToken cancellationToken)
    {
        var options = await session.Option.GetAsync();
        var userOptions = await session.UserOption.GetAsync();
        var user = await session.User.GetAsync();
        
        var queryable = dbContext
            .Users
            .AsNoTracking()
            .AsExpandable() 
            .Where(queryFilter.Where(query));
        
        var total = await queryable
            .CountAsync(cancellationToken);
        var result = await queryable
            .Skip((query.PageNo - 1) * query.PageSize)
            .Take(query.PageSize)       
            .Select(queryFilter.Selector)
            .ToArrayAsync(cancellationToken: cancellationToken);
        
        return await PaginatedResult<UserDto>.SuccessAsync(total, result, query.PageNo, query.PageSize);
    }
}