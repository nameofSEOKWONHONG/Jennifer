using FluentValidation;

namespace Jennifer.Account.Application.Auth.Commands.ExternalOAuth;

internal sealed class ExternalOAuthCommandValidator : AbstractValidator<ExternalOAuthCommand>
{
    public ExternalOAuthCommandValidator()
    {
        RuleFor(m => m.Provider).NotEmpty();
        RuleFor(m => m.AccessToken).NotEmpty();
    }
}