using FluentValidation;

namespace Jennifer.Account.Application.Auth.Commands.ExternalOAuth;

/// <summary>
/// Validator for external OAuth authentication commands
/// Validates that both Provider and AccessToken fields are not empty
/// </summary>
public sealed class ExternalOAuthCommandValidator : AbstractValidator<ExternalOAuthCommand>
{
    public ExternalOAuthCommandValidator()
    {
        RuleFor(m => m.Provider).NotEmpty();
        RuleFor(m => m.AccessToken).NotEmpty(); 
    }
}