using Jennifer.Account.Application.Users.Queries;
using Jennifer.Infrastructure.Database;
using Jennifer.Infrastructure.Extenstions;
using Jennifer.Infrastructure.Session;
using Jennifer.Infrastructure.Session.Abstracts;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Account.Application.Users.Commands;

public sealed class RemoveUserCommandHandler(
    ISessionContext session,
    IUserQueryFilter queryFilter,
    JenniferDbContext dbContext): ICommandHandler<RemoveUserCommand, Result>
{
    public async ValueTask<Result> Handle(RemoveUserCommand command, CancellationToken cancellationToken)
    {
        var user = await dbContext
            .Users
            .Where(queryFilter.Where(command))
            .FirstAsync(cancellationToken);

        await user.AssignSession(session);
        
        user.IsDelete = true;

        await dbContext.SaveChangesAsync(cancellationToken);
        
        await session.User.ClearAsync();

        return await Result.SuccessAsync();
    }
}