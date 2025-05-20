using FluentValidation;

namespace Jennifer.Jwt.Application.Users.Commands;

public class RemoveUserCommandValidator : AbstractValidator<RemoveUserCommand>
{
    public RemoveUserCommandValidator()
    {
        RuleFor(m => m.UserId).NotEmpty();
    }
}