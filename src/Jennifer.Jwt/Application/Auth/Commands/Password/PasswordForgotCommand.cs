using FluentValidation;
using Jennifer.Jwt.Abstractions.Messaging;
using Jennifer.Jwt.Application.Auth.Services.Abstracts;
using Jennifer.Jwt.Application.Auth.Services.Contracts;
using Jennifer.Jwt.Models.Contracts;
using Jennifer.SharedKernel;
using Microsoft.AspNetCore.Http;

namespace Jennifer.Jwt.Application.Auth.Commands.Password;

//PasswordForgot은 비로그인 상태에서 암호변경

public sealed record PasswordForgotCommand(string Email):ICommand<IResult>;

public class PasswordForgotCommandHandler(
    IVerifyCodeByEmailSendService verifyCodeByEmailSendService
    ): ICommandHandler<PasswordForgotCommand, IResult>
{
    public async Task<Result<IResult>> HandleAsync(PasswordForgotCommand command, CancellationToken cancellationToken)
    {
        await verifyCodeByEmailSendService.HandleAsync(
            new VerifyCodeByEmailSendRequest(command.Email, ENUM_EMAIL_VERIFICATION_TYPE.PASSWORD_FORGOT),
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