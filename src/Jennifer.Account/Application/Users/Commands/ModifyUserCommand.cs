using eXtensionSharp;
using Jennifer.Account.Application.Auth.Contracts;
using Jennifer.Account.Data;
using Jennifer.Account.Session.Abstracts;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Account.Application.Users.Commands;

internal sealed record ModifyUserCommand(UserDto userDto): ICommand<Result>;

internal sealed class ModifyUserCommandHandler(ISessionContext context,
    JenniferDbContext dbContext): ICommandHandler<ModifyUserCommand, Result>
{
    public async ValueTask<Result> Handle(ModifyUserCommand command, CancellationToken cancellationToken)
    {
        var exists = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == command.userDto.Id, cancellationToken: cancellationToken);
        if(exists.xIsEmpty()) return Result.Failure("not found user");
        
        exists.PhoneNumber = command.userDto.PhoneNumber;
        exists.UserName = command.userDto.UserName;
        exists.ConcurrencyStamp = Guid.NewGuid().ToString();
        
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}