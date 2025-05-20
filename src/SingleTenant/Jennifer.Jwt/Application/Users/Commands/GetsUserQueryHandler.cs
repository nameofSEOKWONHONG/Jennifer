using System.Data.Entity;
using eXtensionSharp;
using Jennifer.Infrastructure.Abstractions.Messaging;
using Jennifer.Jwt.Application.Auth.Contracts;
using Jennifer.Jwt.Data;
using Jennifer.Jwt.Session.Abstracts;
using Jennifer.SharedKernel;
using LinqKit;

namespace Jennifer.Jwt.Application.Users.Commands;

public class GetsUserQueryHandler(ISessionContext context,
    IUserQueryFilter queryFilter) : IQueryHandler<GetsUserQuery, PagingResult<UserDto>>
{
    public async Task<Result<PagingResult<UserDto>>> HandleAsync(GetsUserQuery query, CancellationToken cancellationToken)
    {   
        var queryable = context.ApplicationDbContext.xAs<JenniferDbContext>()
            .Users.AsNoTracking()
            .AsExpandableEFCore()
            .Where(queryFilter.Where(query));
        
        var total = await queryable.CountAsync(cancellationToken);
        var result = await queryable
            .Skip((query.PageNo - 1) * query.PageSize)
            .Take(query.PageSize)       
            .Select(queryFilter.Selector)
            .ToArrayAsync(cancellationToken: cancellationToken);
        
        return PagingResult<UserDto>.Create(total, result, query.PageNo, query.PageSize);
    }
}