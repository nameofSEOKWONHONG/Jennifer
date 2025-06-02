using Jennifer.Domain.Accounts;
using Jennifer.Infrastructure.Database;
using Jennifer.Infrastructure.Session;
using Jennifer.SharedKernel;
using Mediator;

namespace Jennifer.Account.Application.Roles.Commands;

public sealed class CreateRoleCommandHandler(JenniferDbContext dbContext,
    ISessionContext session) : ICommandHandler<CreateRoleCommand, Result<Guid>>
{
    public async ValueTask<Result<Guid>> Handle(CreateRoleCommand command, CancellationToken cancellationToken)
    {
        var item = Role.Create(command.RoleName);
        
        dbContext.Roles.Add(item);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        await session.User.ClearAsync();
        
        return await Result<Guid>.SuccessAsync(item.Id);
    }
}
