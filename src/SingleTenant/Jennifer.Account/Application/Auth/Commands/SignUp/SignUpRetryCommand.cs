using Jennifer.Account.Application.Auth.Contracts;
using Jennifer.Account.Application.Auth.Services.Abstracts;
using Jennifer.Account.Models.Contracts;
using Jennifer.SharedKernel;
using Mediator;

namespace Jennifer.Account.Application.Auth.Commands.SignUp;

public sealed record SignUpRetryRequest(string Email);
public sealed record SignUpRetryCommand(string Email):ICommand<Result>;

public class SignUpRetryCommandHandler(IVerifyCodeSendEmailService sendVerifyCodeService)
    : ICommandHandler<SignUpRetryCommand, Result>
{
    public async ValueTask<Result> Handle(SignUpRetryCommand command, CancellationToken cancellationToken)
    {
        var result = await sendVerifyCodeService
            .HandleAsync(new VerifyCodeSendEmailRequest(command.Email, command.Email, ENUM_EMAIL_VERIFICATION_TYPE.SIGN_UP_BEFORE), cancellationToken);

        if (!result.IsSuccess) return Result.Failure("signup retry fail");
        
        return Result.Success();
    }
}