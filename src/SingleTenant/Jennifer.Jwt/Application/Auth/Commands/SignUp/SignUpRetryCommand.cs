using Jennifer.Infrastructure.Abstractions.Messaging;
using Jennifer.Jwt.Application.Auth.Services.Abstracts;
using Jennifer.Jwt.Application.Auth.Services.Contracts;
using Jennifer.Jwt.Models.Contracts;
using Jennifer.SharedKernel;
using Microsoft.AspNetCore.Http;

namespace Jennifer.Jwt.Application.Auth.Commands.SignUp;

public sealed record SignUpRetryRequest(string Email);
public sealed record SignUpRetryCommand(string Email):ICommand<bool>;

public class SignUpRetryCommandHandler(IVerifyCodeSendEmailService sendVerifyCodeService): ICommandHandler<SignUpRetryCommand, bool>
{
    public async Task<Result<bool>> HandleAsync(SignUpRetryCommand command, CancellationToken cancellationToken)
    {
        var result = await sendVerifyCodeService
            .HandleAsync(new VerifyCodeSendEmailRequest(command.Email, command.Email, ENUM_EMAIL_VERIFICATION_TYPE.SIGN_UP_BEFORE), cancellationToken);

        return result.IsSuccess;
    }
}