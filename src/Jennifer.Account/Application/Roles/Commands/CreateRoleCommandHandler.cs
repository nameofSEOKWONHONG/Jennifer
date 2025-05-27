using Jennifer.Account.Data;
using Jennifer.Account.Models;
using Jennifer.SharedKernel;
using Mediator;

namespace Jennifer.Account.Application.Roles.Commands;

internal sealed class CreateRoleCommandHandler(JenniferDbContext dbContext) : ICommandHandler<CreateRoleCommand, Result<Guid>>
{
    public async ValueTask<Result<Guid>> Handle(CreateRoleCommand command, CancellationToken cancellationToken)
    {
        var item = Role.Create(command.RoleName);
        
        dbContext.Roles.Add(item);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return await Result<Guid>.SuccessAsync(item.Id);
    }
}
