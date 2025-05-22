using FluentValidation;

namespace Jennifer.Account.Application.Auth.Commands.SignUp;

public class SignUpCommandValidator: AbstractValidator<SignUpCommand>
{
    public SignUpCommandValidator()
    {
        RuleFor(m => m.Email).NotEmpty().EmailAddress();
        RuleFor(m => m.Password).NotEmpty().MinimumLength(8);
        RuleFor(m => m.PhoneNumber).NotEmpty().MaximumLength(20);
        RuleFor(m => m.Type).NotEmpty();
        RuleFor(m => m.UserName).NotEmpty();
    }
}