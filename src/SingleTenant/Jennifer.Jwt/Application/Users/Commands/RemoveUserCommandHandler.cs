using eXtensionSharp;
using Jennifer.Infrastructure.Abstractions.Messaging;
using Jennifer.Jwt.Data;
using Jennifer.Jwt.Session.Abstracts;
using Jennifer.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Jwt.Application.Users.Commands;

public class RemoveUserCommandHandler(ISessionContext context,
    JenniferDbContext dbContext,
    IUserQueryFilter queryFilter): ICommandHandler<RemoveUserCommand>
{
    public async Task<Result> HandleAsync(RemoveUserCommand command, CancellationToken cancellationToken)
    {
        var user = await dbContext
            .Users
            .Where(queryFilter.Where(command))
            .FirstAsync(cancellationToken);

        user.IsDelete = true;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}