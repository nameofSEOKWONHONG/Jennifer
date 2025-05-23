using FluentValidation;

namespace Jennifer.Account.Application.Users.Commands;

internal sealed class RemoveUserCommandValidator : AbstractValidator<RemoveUserCommand>
{
    public RemoveUserCommandValidator()
    {
        RuleFor(m => m.UserId).NotEmpty();
    }
}