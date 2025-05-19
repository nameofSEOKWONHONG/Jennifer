using Jennifer.Jwt.Abstractions.Messaging;
using Jennifer.Jwt.Application.Auth.Services.Abstracts;
using Jennifer.Jwt.Application.Auth.Services.Contracts;
using Jennifer.Jwt.Models.Contracts;
using Jennifer.SharedKernel;
using Microsoft.AspNetCore.Http;

namespace Jennifer.Jwt.Application.Auth.Commands.SignUp;

public sealed record SignUpRetryRequest(string Email);
public sealed record SignUpRetryCommand(string Email):ICommand<IResult>;

public class SignUpRetryCommandHandler(IVerifyCodeSendEmailService sendVerifyCodeService): ICommandHandler<SignUpRetryCommand, IResult>
{
    public async Task<Result<IResult>> HandleAsync(SignUpRetryCommand command, CancellationToken cancellationToken)
    {
        await sendVerifyCodeService
            .HandleAsync(new VerifyCodeSendEmailRequest(command.Email, command.Email, ENUM_EMAIL_VERIFICATION_TYPE.SIGN_UP_BEFORE), cancellationToken);

        return TypedResults.Ok();
    }
}