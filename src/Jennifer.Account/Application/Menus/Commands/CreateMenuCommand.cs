using FluentValidation;
using Jennifer.Account.Application.Menus.Queries;
using Jennifer.Domain.Common;
using Jennifer.Infrastructure.Database;
using Jennifer.Infrastructure.Session;
using Jennifer.SharedKernel;
using Jennifer.SharedKernel.Account.Menu;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Account.Application.Menus.Commands;

[UseTransaction]
public sealed record CreateMenuCommand(MenuDto menuDto):ICommand<Result<Guid>>;

public class CreateMenuCommandValidator : AbstractValidator<CreateMenuCommand>
{
    public CreateMenuCommandValidator()
    {
        RuleFor(m => m.menuDto).NotNull();
        RuleFor(m => m.menuDto).SetValidator(new MenuDtoValidator());
    }
}

public class MenuDtoValidator : AbstractValidator<MenuDto>
{
    public MenuDtoValidator()
    {
        RuleFor(m => m.Name).NotEmpty()
            .MaximumLength(100);
        RuleFor(m => m.ParentId).NotEmpty();
        RuleFor(m => m.Icon).MaximumLength(255);
        RuleFor(m => m.Url).NotEmpty()
            .MaximumLength(4000);
        RuleFor(m => m.Order).NotEmpty();
    }
}

public sealed class CreateMenuCommandHandler(JenniferDbContext dbContext) : ICommandHandler<CreateMenuCommand, Result<Guid>>
{
    public async ValueTask<Result<Guid>> Handle(CreateMenuCommand command, CancellationToken cancellationToken)
    {
        var exists = await dbContext.Menus.Where(m => m.Id == command.menuDto.Id)
            .AnyAsync(cancellationToken: cancellationToken);
        if (exists) return await Result<Guid>.FailureAsync("menu already exists.");
        
        var newItem = Menu.Create(command.menuDto.Name, command.menuDto.Icon, command.menuDto.Url, command.menuDto.ParentId, command.menuDto.Order);
        await dbContext.Menus.AddAsync(newItem, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return await Result<Guid>.SuccessAsync(newItem.Id);
    }
}