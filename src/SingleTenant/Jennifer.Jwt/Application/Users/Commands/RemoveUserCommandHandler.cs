using eXtensionSharp;
using FluentValidation;
using Jennifer.Infrastructure.Abstractions.Messaging;
using Jennifer.Jwt.Data;
using Jennifer.Jwt.Session;
using Jennifer.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Jwt.Application.Users.Commands;

public sealed record RemoveUserCommand(Guid UserId) : ICommand;
public class RemoveUserCommandHandler(ISessionContext context): ICommandHandler<RemoveUserCommand>
{
    public async Task<Result> HandleAsync(RemoveUserCommand command, CancellationToken cancellationToken)
    {
        var dbContext = context.xAs<JenniferDbContext>();
        var user = await dbContext
            .Users
            .FirstAsync(m => m.Id == command.UserId, cancellationToken);

        user.IsDelete = true;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

public class RemoveUserCommandValidator : AbstractValidator<RemoveUserCommand>
{
    public RemoveUserCommandValidator()
    {
        RuleFor(m => m.UserId).NotEmpty();
    }
}