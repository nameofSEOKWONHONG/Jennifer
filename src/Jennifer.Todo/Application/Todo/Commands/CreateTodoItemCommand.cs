using FluentValidation;
using Jennifer.SharedKernel;
using Jennifer.Todo.Application.Todo.Contracts;
using Mediator;

namespace Jennifer.Todo.Application.Todo.Commands;

/// <summary>
/// Command to create a new todo item
/// </summary>
public sealed record CreateTodoItemCommand(TodoItemDto Item): ICommand<Result<Guid>>;

public sealed class CreateTodoItemCommandValidator : AbstractValidator<CreateTodoItemCommand>
{
    public CreateTodoItemCommandValidator()
    {
        RuleFor(m => m.Item)
            .NotEmpty();
        
        RuleFor(m => m.Item)
            .SetValidator(new TodoItemDtoValidator());
    }
}