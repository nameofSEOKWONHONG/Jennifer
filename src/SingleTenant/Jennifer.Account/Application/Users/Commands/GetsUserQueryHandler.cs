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

public class GetsUserQueryHandler(ISessionContext context,
    IUserQueryFilter queryFilter) : IQueryHandler<GetsUserQuery, PagingResult<UserDto[]>>
{
    public async ValueTask<PagingResult<UserDto[]>> Handle(GetsUserQuery query, CancellationToken cancellationToken)
    {
        var queryable = context.ApplicationDbContext.xAs<JenniferDbContext>()
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