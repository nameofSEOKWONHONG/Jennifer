using eXtensionSharp;
using Jennifer.Domain.Accounts;
using Jennifer.Infrastructure.Database;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Account.Application.Users.Commands;

public sealed class AddOrUpdateUserRoleCommandHandler(JenniferDbContext dbContext): ICommandHandler<AddOrUpdateUserRoleCommand, Result>
{
    public async ValueTask<Result> Handle(AddOrUpdateUserRoleCommand command, CancellationToken cancellationToken)
    {
        var exists = await dbContext.UserRoles.FirstOrDefaultAsync(m => m.UserId == command.UserId, cancellationToken: cancellationToken);
        if (exists.xIsEmpty())
        {
            var role = new UserRole()
            {
                UserId = command.UserId,
                RoleId = command.RoleId
            };
            await dbContext.UserRoles.AddAsync(role, cancellationToken);
        }
        else
        {
            exists.RoleId = command.RoleId;
            dbContext.UserRoles.Update(exists);
        }
        
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return await Result.SuccessAsync();
    }
}