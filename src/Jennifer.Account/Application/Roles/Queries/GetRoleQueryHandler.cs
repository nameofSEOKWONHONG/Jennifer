﻿using Jennifer.Infrastructure.Database;
using Jennifer.SharedKernel;
using Jennifer.SharedKernel.Account.Role;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Account.Application.Roles.Queries;

public sealed class GetRoleQueryHandler(JenniferDbContext dbContext): IQueryHandler<GetRoleQuery, Result<RoleDto>>
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
        
        return await Result<RoleDto>.SuccessAsync(result);
    }
}