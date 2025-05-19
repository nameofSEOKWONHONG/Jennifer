using FluentValidation;
using Jennifer.Infrastructure.Abstractions.Messaging;
using Jennifer.Jwt.Application.Auth.Services.Abstracts;
using Jennifer.Jwt.Application.Auth.Services.Contracts;
using Jennifer.Jwt.Models.Contracts;
using Jennifer.SharedKernel;
using Microsoft.AspNetCore.Http;

namespace Jennifer.Jwt.Application.Auth.Commands.Password;

//PasswordForgot은 비로그인 상태에서 암호변경

public sealed record PasswordForgotRequest(string Email, string UserName);
public sealed record PasswordForgotCommand(string Email, string UserName):ICommand<IResult>;

public class PasswordForgotCommandHandler(
    IVerifyCodeSendEmailService verifyCodeSendEmailService
    ): ICommandHandler<PasswordForgotCommand, IResult>
{
    public async Task<Result<IResult>> HandleAsync(PasswordForgotCommand command, CancellationToken cancellationToken)
    {
        await verifyCodeSendEmailService.HandleAsync(
            new VerifyCodeSendEmailRequest(command.Email, command.UserName, ENUM_EMAIL_VERIFICATION_TYPE.PASSWORD_FORGOT),
            cancellationToken);
        return TypedResults.Ok();
    }
}

public class PasswordForgotCommandValidator : AbstractValidator<PasswordForgotCommand>
{
    public PasswordForgotCommandValidator()
    {
        RuleFor(m => m.Email).NotEmpty().EmailAddress();
    }
}