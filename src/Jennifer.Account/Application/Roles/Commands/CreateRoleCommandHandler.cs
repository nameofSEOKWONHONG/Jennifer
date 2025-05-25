using Jennifer.Account.Data;
using Jennifer.Account.Models;
using Jennifer.SharedKernel;
using Mediator;

namespace Jennifer.Account.Application.Roles.Commands;

internal sealed class CreateRoleCommandHandler(JenniferDbContext dbContext) : ICommandHandler<CreateRoleCommand, Result<Guid>>
{
    public async ValueTask<Result<Guid>> Handle(CreateRoleCommand command, CancellationToken cancellationToken)
    {
        var item = new Role()
        {
            Name = command.RoleName,
            NormalizedName = command.RoleName.ToUpper(),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
        };
        
        dbContext.Roles.Add(item);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return Result<Guid>.Success(item.Id);
    }
}
