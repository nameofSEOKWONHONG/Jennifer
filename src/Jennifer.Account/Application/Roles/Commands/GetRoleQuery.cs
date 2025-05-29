using Jennifer.Account.Application.Roles.Contracts;
using Jennifer.Domain.Account;
using Jennifer.Domain.Database;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Account.Application.Roles.Commands;

internal sealed record GetRoleQuery(Guid Id):IQuery<Result<RoleDto>>;
internal sealed class GetRoleQueryHandler(JenniferDbContext dbContext): IQueryHandler<GetRoleQuery, Result<RoleDto>>
{
    public async ValueTask<Result<RoleDto>> Handle(GetRoleQuery query, CancellationToken cancellationToken)
    {
        var result = await dbContext.Roles
            .Include(m => m.RoleClaims)
            .AsNoTracking()
            .Where(m => m.Id == query.Id)
            .Select(m => new RoleDto()
            {
                Id = m.Id,
                Name = m.Name,
                NormalizedName = m.NormalizedName,
                RoleClaims = m.RoleClaims.Select(m => new RoleClaimDto()
                {
                    Id = m.Id,
                    ClaimType = m.ClaimType,
                    ClaimValue = m.ClaimValue
                })
            })
            .FirstOrDefaultAsync(cancellationToken);
        
        return Result<RoleDto>.Success(result);
    }
}