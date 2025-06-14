using Jennifer.Account.Application.Auth.Services.Abstracts;
using Jennifer.Domain.Accounts.Contracts;
using Jennifer.Infrastructure.Abstractions.ServiceCore;
using Jennifer.SharedKernel;
using Jennifer.SharedKernel.Account.Auth;
using Mediator;

namespace Jennifer.Account.Application.Auth.Commands.SignUp;


public sealed record SignUpRetryCommand(string Email):ICommand<Result>;

public sealed class SignUpRetryCommandHandler(
    IServiceExecutionBuilderFactory factory)
    : ICommandHandler<SignUpRetryCommand, Result>
{
    public async ValueTask<Result> Handle(SignUpRetryCommand command, CancellationToken cancellationToken)
    {
        Result result = null;
        
        var builder = factory.Create();
        await builder.Register<IEmailConfirmSendService, EmailConfirmSendRequest, Result>()
            .Request(new EmailConfirmSendRequest(command.Email, command.Email,
                ENUM_EMAIL_VERIFY_TYPE.SIGN_UP_BEFORE.Name))
            .Handle(r => result = r)
            .ExecuteAsync(cancellationToken);

        if (!result.IsSuccess) return await Result.FailureAsync("signup retry fail");
        
        return await Result.SuccessAsync();
    }
}