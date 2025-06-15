using System.Data;
using eXtensionSharp;
using FluentValidation;
using Jennifer.Account.Application.Menus.Queries;
using Jennifer.Infrastructure.Database;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Account.Application.Menus.Commands;

[UseTransaction(IsolationLevel.ReadUncommitted)]
public sealed record ModifyMenuCommand(MenuDto menuDto):ICommand<Result>;

public sealed class ModifyMenuCommandValidator : AbstractValidator<ModifyMenuCommand>
{
    public ModifyMenuCommandValidator()
    {
        RuleFor(m => m.menuDto).NotNull();
        RuleFor(m => m.menuDto).SetValidator(new MenuDtoValidator());       
    }
}

public sealed class ModifyMenuCommandHandler(JenniferDbContext dbContext) : ICommandHandler<ModifyMenuCommand, Result>
{
    public async ValueTask<Result> Handle(ModifyMenuCommand command, CancellationToken cancellationToken)
    {
        var exists = await dbContext.Menus.FirstOrDefaultAsync(m => m.Id == command.menuDto.Id, cancellationToken: cancellationToken);
        if (exists.xIsEmpty()) return await Result<bool>.FailureAsync("not found");
        
        exists.Update(command.menuDto.Name, command.menuDto.Icon, command.menuDto.Url, command.menuDto.IsVisible, command.menuDto.ParentId, command.menuDto.Order);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return await Result.SuccessAsync();       
    }
}