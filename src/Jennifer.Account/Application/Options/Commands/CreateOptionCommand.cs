using eXtensionSharp;
using Jennifer.Domain.Account;
using Jennifer.Domain.Account.Contracts;
using Jennifer.Infrastructure.Database;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Account.Application.Options.Commands;

public sealed record CreateOptionCommand(ENUM_ACCOUNT_OPTION type, string Value): ICommand<Result<int>>;
public sealed class CreateOptionCommandHandler(
    JenniferDbContext dbContext
    ): ICommandHandler<CreateOptionCommand, Result<int>>
{
    public async ValueTask<Result<int>> Handle(CreateOptionCommand command, CancellationToken cancellationToken)
    {
        var exists = await dbContext.Options.Where(m => m.Type == command.type)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        
        if(exists.xIsNotEmpty()) return await Result<int>.FailureAsync("already exists");
        
        var item = Option.Create(type: command.type, value: command.Value);
        await dbContext.Options.AddAsync(item, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return await Result<int>.SuccessAsync(item.Id);       
    }
}