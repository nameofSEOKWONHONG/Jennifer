using FluentValidation;
using Jennifer.Account.Application.Auth.Contracts;
using Jennifer.Account.Application.Auth.Services.Abstracts;
using Jennifer.Account.Models.Contracts;
using Jennifer.SharedKernel;
using Mediator;

namespace Jennifer.Account.Application.Auth.Commands.Password;

//PasswordForgot은 비로그인 상태에서 암호변경

public sealed record PasswordForgotRequest(string Email, string UserName);
public sealed record PasswordForgotCommand(string Email, string UserName):ICommand<Result>;

public class PasswordForgotCommandHandler(
    IVerifyCodeSendEmailService verifyCodeSendEmailService
    ): ICommandHandler<PasswordForgotCommand, Result>
{
    public async ValueTask<Result> Handle(PasswordForgotCommand command, CancellationToken cancellationToken)
    {
        await verifyCodeSendEmailService.HandleAsync(
            new VerifyCodeSendEmailRequest(command.Email, command.UserName, ENUM_EMAIL_VERIFICATION_TYPE.PASSWORD_FORGOT),
            cancellationToken);
        return Result.Success();
    }
}

public class PasswordForgotCommandValidator : AbstractValidator<PasswordForgotCommand>
{
    public PasswordForgotCommandValidator()
    {
        RuleFor(m => m.Email).NotEmpty().EmailAddress();
    }
}