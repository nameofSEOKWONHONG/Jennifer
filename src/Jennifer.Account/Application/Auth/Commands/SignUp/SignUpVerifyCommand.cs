using FluentValidation;
using Jennifer.SharedKernel;
using Mediator;

namespace Jennifer.Account.Application.Auth.Commands.SignUp;

public sealed record SignUpVerifyRequest(Guid UserId, string Code);

public sealed record SignUpVerifyCommand(Guid UserId, string Code):ICommand<Result>;

internal sealed class SignUpVerifyCommandValidator : AbstractValidator<SignUpVerifyCommand>
{
    public SignUpVerifyCommandValidator()
    {
        RuleFor(m => m.UserId).NotEmpty();
        RuleFor(m => m.Code).NotEmpty();
    }
}