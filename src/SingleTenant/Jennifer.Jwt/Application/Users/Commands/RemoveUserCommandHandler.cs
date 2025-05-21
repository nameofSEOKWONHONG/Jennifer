using eXtensionSharp;
using Jennifer.Jwt.Data;
using Jennifer.Jwt.Session.Abstracts;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Jwt.Application.Users.Commands;

public sealed class RemoveUserCommandHandler(ISessionContext context,
    IUserQueryFilter queryFilter): ICommandHandler<RemoveUserCommand, Result>
{
    public async ValueTask<Result> Handle(RemoveUserCommand command, CancellationToken cancellationToken)
    {
        var dbContext = context.ApplicationDbContext.xAs<JenniferDbContext>();
        var user = await dbContext
            .Users
            .Where(queryFilter.Where(command))
            .FirstAsync(cancellationToken);

        user.IsDelete = true;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}