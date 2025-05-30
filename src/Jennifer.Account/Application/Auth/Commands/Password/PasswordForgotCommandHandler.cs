using Jennifer.Account.Application.Auth.Contracts;
using Jennifer.Account.Application.Auth.Services.Abstracts;
using Jennifer.Domain.Accounts.Contracts;
using Jennifer.Infrastructure.Abstractions.ServiceCore;
using Jennifer.SharedKernel;
using Mediator;

namespace Jennifer.Account.Application.Auth.Commands.Password;

public sealed class PasswordForgotCommandHandler(
    IServiceExecutionBuilderFactory factory
): ICommandHandler<PasswordForgotCommand, Result>
{
    public async ValueTask<Result> Handle(PasswordForgotCommand command, CancellationToken cancellationToken)
    {
        Result result = null;
        
        var builder = factory.Create();
        await builder.Register<IVerifyCodeSendEmailService, VerifyCodeSendEmailRequest, Result>()
            .Request(new VerifyCodeSendEmailRequest(command.Email, command.UserName,
                ENUM_EMAIL_VERIFY_TYPE.PASSWORD_FORGOT))
            .Handle(r => result = r)
            .ExecuteAsync(cancellationToken);
        
        return result;
    }
}