using FluentValidation;

namespace Jennifer.Jwt.Application.Auth.Commands.SignUpAdmin;

internal sealed class SignUpAdminCommandValidator : AbstractValidator<SignUpAdminCommand>
{
    public SignUpAdminCommandValidator()
    {
        RuleFor(c => c.Email).NotEmpty().EmailAddress();
        RuleFor(c => c.Password).NotEmpty().MinimumLength(8);
    }
}