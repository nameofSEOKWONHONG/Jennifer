using eXtensionSharp;
using Jennifer.Domain.Accounts;
using Jennifer.Infrastructure.Database;
using Jennifer.Infrastructure.Session;
using Jennifer.Infrastructure.Session.Abstracts;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Account.Application.Options.Commands;

public sealed class CreateOptionCommandHandler(
    ISessionContext session,
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

        await session.Option.ClearAsync();
        
        return await Result<int>.SuccessAsync(item.Id);       
    }
}