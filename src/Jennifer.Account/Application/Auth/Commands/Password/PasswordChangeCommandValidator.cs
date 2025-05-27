using FluentValidation;

namespace Jennifer.Account.Application.Auth.Commands.Password;

internal sealed class PasswordChangeCommandValidator : AbstractValidator<PasswordChangeCommand>
{
    public PasswordChangeCommandValidator()
    {
        RuleFor(m => m.OldPassword).NotEmpty().MinimumLength(8);
        RuleFor(m => m.NewPassword).NotEmpty().MinimumLength(8);
    }
}