using eXtensionSharp;
using Jennifer.Jwt.Application.Auth.Contracts;
using Jennifer.Jwt.Data;
using Jennifer.Jwt.Session.Abstracts;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Jwt.Application.Users.Commands;

public sealed record ModifyUserCommand(UserDto userDto): ICommand<Result>;

public sealed class ModifyUserCommandHandler(ISessionContext context): ICommandHandler<ModifyUserCommand, Result>
{
    public async ValueTask<Result> Handle(ModifyUserCommand command, CancellationToken cancellationToken)
    {
        var dbContext = context.xAs<JenniferDbContext>();
        var exists = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == command.userDto.Id, cancellationToken: cancellationToken);
        if(exists.xIsEmpty()) return Result.Failure("not found user");
        
        exists.PhoneNumber = command.userDto.PhoneNumber;
        exists.UserName = command.userDto.UserName;
        exists.ConcurrencyStamp = Guid.NewGuid().ToString();
        
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}