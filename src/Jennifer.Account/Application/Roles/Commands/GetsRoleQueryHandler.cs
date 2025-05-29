using Jennifer.Account.Application.Roles.Contracts;
using Jennifer.Domain.Account;
using Jennifer.Domain.Database;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Account.Application.Roles.Commands;

internal sealed class GetsRoleQueryHandler(JenniferDbContext dbContext) : IQueryHandler<GetsRoleQuery, PaginatedResult<IEnumerable<RoleDto>>>
{
    public async ValueTask<PaginatedResult<IEnumerable<RoleDto>>> Handle(GetsRoleQuery query, CancellationToken cancellationToken)
    {
        var queryable = dbContext.Roles.AsNoTracking()
            .Where(m => m.NormalizedName == query.RoleName.ToUpper());
        var total = queryable.Count();
        var result = await queryable
            .Select(m => new RoleDto
            {
                Id = m.Id, 
                Name = m.Name, 
                NormalizedName = m.NormalizedName
            })
            .Skip((query.PageNo - 1) * query.PageSize)
            .Take(query.PageSize)       
            .ToArrayAsync(cancellationToken);
        
        return await PaginatedResult<IEnumerable<RoleDto>>.SuccessAsync(total, result, query.PageNo, query.PageSize);
    }
}