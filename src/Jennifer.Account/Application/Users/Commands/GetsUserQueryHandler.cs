using Jennifer.Account.Application.Auth.Contracts;
using Jennifer.Account.Application.Users.Filters;
using Jennifer.Account.Data;
using Jennifer.SharedKernel;
using LinqKit;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Account.Application.Users.Commands;

internal sealed class GetsUserQueryHandler(
    IUserQueryFilter queryFilter,
    JenniferDbContext dbContext) : IQueryHandler<GetsUserQuery, PagingResult<UserDto[]>>
{
    public async ValueTask<PagingResult<UserDto[]>> Handle(GetsUserQuery query, CancellationToken cancellationToken)
    {
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
        
        return PagingResult<UserDto[]>.Success(total, result, query.PageNo, query.PageSize);
    }
}