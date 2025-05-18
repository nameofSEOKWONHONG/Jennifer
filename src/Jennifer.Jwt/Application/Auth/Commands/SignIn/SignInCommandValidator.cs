using FluentValidation;

namespace Jennifer.Jwt.Application.Auth.Commands.SignIn;

public class SignInCommandValidator: AbstractValidator<SignInCommand>
{
    public SignInCommandValidator()
    {
        RuleFor(m => m.Email).NotEmpty().EmailAddress();
        RuleFor(m => m.Password).NotEmpty();
    }
}