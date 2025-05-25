using Jennifer.Account.Application.Roles.Contracts;
using Jennifer.Account.Data;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Account.Application.Roles.Commands;

internal sealed class GetsRoleQueryHandler(JenniferDbContext dbContext) : IQueryHandler<GetsRoleQuery, PagingResult<IEnumerable<RoleDto>>>
{
    public async ValueTask<PagingResult<IEnumerable<RoleDto>>> Handle(GetsRoleQuery query, CancellationToken cancellationToken)
    {
        var queryable = dbContext.Roles.AsNoTracking()
            .Where(m => m.NormalizedName == query.RoleName.ToUpper());
        var total = queryable.Count();
        var result = await queryable
            .Select(m => new RoleDto(m.Id, m.Name, m.NormalizedName))
            .Skip((query.PageNo - 1) * query.PageSize)
            .Take(query.PageSize)       
            .ToArrayAsync(cancellationToken);
        
        return PagingResult<IEnumerable<RoleDto>>.Success(total, result, query.PageNo, query.PageSize);
    }
}