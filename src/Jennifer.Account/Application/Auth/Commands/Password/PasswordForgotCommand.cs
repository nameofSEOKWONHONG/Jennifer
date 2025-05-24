using FluentValidation;
using Jennifer.Account.Application.Auth.Contracts;
using Jennifer.Account.Application.Auth.Services.Abstracts;
using Jennifer.Account.Models.Contracts;
using Jennifer.Infrastructure.Abstractions.ServiceCore;
using Jennifer.SharedKernel;
using Mediator;

namespace Jennifer.Account.Application.Auth.Commands.Password;

//PasswordForgot은 비로그인 상태에서 암호변경

public sealed record PasswordForgotRequest(string Email, string UserName);
public sealed record PasswordForgotCommand(string Email, string UserName):ICommand<Result>;

internal sealed class PasswordForgotCommandHandler(
    IServiceExecutionBuilderFactory factory
    ): ICommandHandler<PasswordForgotCommand, Result>
{
    public async ValueTask<Result> Handle(PasswordForgotCommand command, CancellationToken cancellationToken)
    {
        Result result = null;
        var builder = factory.Create();
        builder.Register<IVerifyCodeSendEmailService, VerifyCodeSendEmailRequest, Result>()
            .Request(new VerifyCodeSendEmailRequest(command.Email, command.UserName,
                ENUM_EMAIL_VERIFICATION_TYPE.PASSWORD_FORGOT))
            .Handle(r => result = r);
        return result;
    }
}

internal sealed class PasswordForgotCommandValidator : AbstractValidator<PasswordForgotCommand>
{
    public PasswordForgotCommandValidator()
    {
        RuleFor(m => m.Email).NotEmpty().EmailAddress();
    }
}