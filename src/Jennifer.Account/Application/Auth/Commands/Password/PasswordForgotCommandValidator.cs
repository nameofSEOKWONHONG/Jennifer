using FluentValidation;

namespace Jennifer.Account.Application.Auth.Commands.Password;

internal sealed class PasswordForgotCommandValidator : AbstractValidator<PasswordForgotCommand>
{
    public PasswordForgotCommandValidator()
    {
        RuleFor(m => m.Email).NotEmpty().EmailAddress();
    }
}