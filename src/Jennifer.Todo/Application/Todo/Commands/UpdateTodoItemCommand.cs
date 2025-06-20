using FluentValidation;
using Jennifer.SharedKernel;
using Jennifer.SharedKernel.Todo;
using Mediator;

namespace Jennifer.Todo.Application.Todo.Commands;

/// <summary>
/// Command to update an existing todo item
/// </summary>
public sealed record UpdateTodoItemCommand(TodoItemDto Item): ICommand<Result>;

public sealed class UpdateTodoItemCommandValidator : AbstractValidator<UpdateTodoItemCommand>
{
    public UpdateTodoItemCommandValidator()
    {
        RuleFor(m => m.Item)
            .NotEmpty();
        
        RuleFor(m => m.Item)
            .SetValidator(new TodoItemDtoValidator());
    }
}