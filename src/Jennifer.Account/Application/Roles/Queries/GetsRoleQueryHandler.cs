using Jennifer.Infrastructure.Database;
using Jennifer.SharedKernel;
using Jennifer.SharedKernel.Account.Role;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Account.Application.Roles.Queries;

public sealed class GetsRoleQueryHandler(JenniferDbContext dbContext) : IQueryHandler<GetsRoleQuery, PaginatedResult<RoleDto>>
{
    public async ValueTask<PaginatedResult<RoleDto>> Handle(GetsRoleQuery query, CancellationToken cancellationToken)
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
        
        return await PaginatedResult<RoleDto>.SuccessAsync(total, result, query.PageNo, query.PageSize);
    }
}