using FluentValidation;

namespace Jennifer.Account.Application.Auth.Commands.Password;

internal class PasswordForgotVerifyCommandValidator : AbstractValidator<PasswordForgotVerifyCommand>
{
    public PasswordForgotVerifyCommandValidator()
    {
        RuleFor(m => m.Email).NotEmpty().EmailAddress();
        RuleFor(m => m.Code).NotEmpty().MinimumLength(6).MaximumLength(6);
    }
}