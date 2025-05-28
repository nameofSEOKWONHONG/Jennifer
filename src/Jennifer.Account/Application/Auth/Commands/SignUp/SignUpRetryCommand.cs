using Jennifer.Account.Application.Auth.Contracts;
using Jennifer.Account.Application.Auth.Services.Abstracts;
using Jennifer.Account.Models.Contracts;
using Jennifer.Infrastructure.Abstractions.ServiceCore;
using Jennifer.SharedKernel;
using Mediator;

namespace Jennifer.Account.Application.Auth.Commands.SignUp;


public sealed record SignUpRetryCommand(string Email):ICommand<Result>;

internal sealed class SignUpRetryCommandHandler(
    IServiceExecutionBuilderFactory factory)
    : ICommandHandler<SignUpRetryCommand, Result>
{
    public async ValueTask<Result> Handle(SignUpRetryCommand command, CancellationToken cancellationToken)
    {
        Result result = null;
        
        var builder = factory.Create();
        await builder.Register<IVerifyCodeSendEmailService, VerifyCodeSendEmailRequest, Result>()
            .Request(new VerifyCodeSendEmailRequest(command.Email, command.Email,
                ENUM_EMAIL_VERIFICATION_TYPE.SIGN_UP_BEFORE))
            .Handle(r => result = r)
            .ExecuteAsync(cancellationToken);

        if (!result.IsSuccess) return await Result.FailureAsync("signup retry fail");
        
        return await Result.SuccessAsync();
    }
}