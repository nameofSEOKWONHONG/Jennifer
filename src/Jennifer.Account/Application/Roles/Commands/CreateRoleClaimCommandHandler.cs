using Jennifer.Domain.Accounts;
using Jennifer.Infrastructure.Database;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Account.Application.Roles.Commands;

public sealed class CreateRoleClaimCommandHandler(JenniferDbContext dbContext) : ICommandHandler<CreateRoleClaimCommand, Result>
{
    public async ValueTask<Result> Handle(CreateRoleClaimCommand command, CancellationToken cancellationToken)
    {
        await dbContext.RoleClaims.Where(m => m.RoleId == command.RoleId)
            .ExecuteDeleteAsync(cancellationToken: cancellationToken);
        
        var list = new List<RoleClaim>();
        foreach (var createRoleClaimRequest in command.requests)
        {
            var item = RoleClaim.Create(command.RoleId, createRoleClaimRequest.ClaimType, createRoleClaimRequest.ClaimValue);
            list.Add(item);
        }

        await dbContext.RoleClaims.AddRangeAsync(list, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return await Result.SuccessAsync();
    }
}