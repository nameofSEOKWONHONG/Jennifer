using Jennifer.Account.Data;
using Jennifer.Account.Models;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Account.Application.Roles.Commands;

internal sealed class CreateRoleClaimCommandHandler(JenniferDbContext dbContext) : ICommandHandler<CreateRoleClaimCommand, Result>
{
    public async ValueTask<Result> Handle(CreateRoleClaimCommand command, CancellationToken cancellationToken)
    {
        await dbContext.RoleClaims.Where(m => m.RoleId == command.RoleId)
            .ExecuteDeleteAsync(cancellationToken: cancellationToken);
        
        var list = new List<RoleClaim>();
        foreach (var createRoleClaimRequest in command.requests)
        {
            var roleClaim = new RoleClaim()
            {
                RoleId = command.RoleId,
                ClaimType = createRoleClaimRequest.ClaimType,
                ClaimValue = createRoleClaimRequest.ClaimValue,
            };
            list.Add(roleClaim);
        }

        await dbContext.RoleClaims.AddRangeAsync(list, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}