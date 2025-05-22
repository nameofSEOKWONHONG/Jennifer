using FluentValidation;

namespace Jennifer.Account.Application.Auth.Commands.SignUp;

internal sealed class SignUpAdminCommandValidator : AbstractValidator<SignUpAdminCommand>
{
    public SignUpAdminCommandValidator()
    {
        RuleFor(c => c.Email).NotEmpty().EmailAddress();
        RuleFor(c => c.Password).NotEmpty().MinimumLength(8);
        RuleFor(c => c.UserName).NotEmpty().MaximumLength(30);
        RuleFor(c => c.PhoneNumber).NotEmpty().MaximumLength(20);
    }
}