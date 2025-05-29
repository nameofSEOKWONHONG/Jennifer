using Jennifer.Account.Application.Users.Filters;
using Jennifer.Account.Session.Abstracts;
using Jennifer.Domain.Account;
using Jennifer.Infrastructure.Database;
using Jennifer.Infrastructure.Extenstions;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Account.Application.Users.Commands;

internal sealed class RemoveUserCommandHandler(
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

        return await Result.SuccessAsync();
    }
}