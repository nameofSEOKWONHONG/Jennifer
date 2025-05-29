using Jennifer.Account.Application.Auth.Contracts;
using Jennifer.Account.Application.Users.Filters;
using Jennifer.Account.Session.Abstracts;
using Jennifer.Domain.Account;
using Jennifer.Infrastructure.Database;
using Jennifer.SharedKernel;
using LinqKit;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Account.Application.Users.Commands;

internal sealed class GetsUserQueryHandler(
    IUserQueryFilter queryFilter,
    JenniferDbContext dbContext) : IQueryHandler<GetsUserQuery, PaginatedResult<UserDto[]>>
{
    public async ValueTask<PaginatedResult<UserDto[]>> Handle(GetsUserQuery query, CancellationToken cancellationToken)
    {
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
        
        return await PaginatedResult<UserDto[]>.SuccessAsync(total, result, query.PageNo, query.PageSize);
    }
}