using eXtensionSharp;
using Jennifer.Infrastructure.Database;
using Jennifer.Infrastructure.Extenstions;
using Jennifer.Infrastructure.Session;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Account.Application.Users.Commands;

public sealed class ModifyUserCommandHandler(
    ISessionContext session,
    JenniferDbContext dbContext): ICommandHandler<ModifyUserCommand, Result>
{
    public async ValueTask<Result> Handle(ModifyUserCommand command, CancellationToken cancellationToken)
    {
        var exists = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == command.UserId, cancellationToken: cancellationToken);
        if(exists.xIsEmpty()) return await Result.FailureAsync("not found user");

        await exists.AssignSession(session);
        
        exists.PhoneNumber = command.PhoneNumber;
        exists.UserName = command.UserName;
        exists.NormalizedUserName = command.UserName.ToUpper();
        exists.ConcurrencyStamp = Guid.NewGuid().ToString();
        
        await dbContext.SaveChangesAsync(cancellationToken);

        await session.User.ClearAsync();

        return await Result.SuccessAsync();
    }
}
