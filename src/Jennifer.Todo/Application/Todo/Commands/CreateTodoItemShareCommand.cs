using FluentValidation;
using Jennifer.SharedKernel;
using Mediator;

namespace Jennifer.Todo.Application.Todo.Commands;

public sealed record CreateTodoItemShareCommand(Guid TodoItemId, Guid[] SharedUsers) : ICommand<Result>;

public sealed class CreateTodoItemShareCommandValidator : AbstractValidator<CreateTodoItemShareCommand>
{
    public CreateTodoItemShareCommandValidator()
    {
        RuleFor(m => m.TodoItemId)
            .NotEmpty();

        RuleFor(m => m.SharedUsers)
            .ForEach(m => m.NotEmpty());
    }
}