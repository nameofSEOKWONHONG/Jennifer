
using FluentValidation;
using Jennifer.SharedKernel;
using Mediator;


namespace Jennifer.Todo.Application.Todo.Commands;

/// <summary>
/// Command to remove a todo item
/// </summary>
public sealed record RemoveTodoItemCommand(Guid Id):ICommand<Result>;

public sealed class RemoveTodoItemCommandValidator : AbstractValidator<RemoveTodoItemCommand>
{
    public RemoveTodoItemCommandValidator()
    {
        RuleFor(m => m.Id)
            .NotEmpty();
    }
}