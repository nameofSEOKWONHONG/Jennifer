using FluentValidation;
using Jennifer.SharedKernel;
using Mediator;

namespace Jennifer.Account.Application.Users.Commands;

public sealed record RemoveUserCommand(Guid UserId) : ICommand<Result>;

public sealed class RemoveUserCommandValidator : AbstractValidator<RemoveUserCommand>
{
    public RemoveUserCommandValidator()
    {
        RuleFor(m => m.UserId).NotEmpty();
    }
}