using eXtensionSharp;
using Jennifer.Account.Data;
using Jennifer.Account.Models;
using Jennifer.Account.Models.Contracts;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Account.Application.Options;

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