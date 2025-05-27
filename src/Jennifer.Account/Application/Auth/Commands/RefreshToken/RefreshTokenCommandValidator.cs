using FluentValidation;

namespace Jennifer.Account.Application.Auth.Commands.RefreshToken;

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(m => m.Token).NotEmpty();
    }
}