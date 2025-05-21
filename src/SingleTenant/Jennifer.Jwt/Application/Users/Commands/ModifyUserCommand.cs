using eXtensionSharp;
using Jennifer.Infrastructure.Abstractions.Messaging;
using Jennifer.Jwt.Application.Auth.Contracts;
using Jennifer.Jwt.Data;
using Jennifer.Jwt.Session.Abstracts;
using Jennifer.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Jwt.Application.Users.Commands;

public sealed record ModifyUserCommand(UserDto userDto): ICommand<bool>;

public class ModifyUserCommandHandler(ISessionContext context, JenniferDbContext dbContext): ICommandHandler<ModifyUserCommand, bool>
{
    public async Task<Result<bool>> HandleAsync(ModifyUserCommand command, CancellationToken cancellationToken)
    {
        var exists = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == command.userDto.Id, cancellationToken: cancellationToken);
        if(exists.xIsEmpty()) return Result.Failure<bool>(Error.NotFound("", "Not Found User"));
        
        exists.PhoneNumber = command.userDto.PhoneNumber;
        exists.UserName = command.userDto.UserName;
        exists.ConcurrencyStamp = Guid.NewGuid().ToString();
        
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}