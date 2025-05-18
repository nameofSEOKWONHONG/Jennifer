using FluentValidation;
using Jennifer.Jwt.Abstractions.Messaging;
using Jennifer.Jwt.Application.Auth.Services.Abstracts;
using Jennifer.Jwt.Application.Auth.Services.Contracts;
using Jennifer.Jwt.Models.Contracts;
using Jennifer.Jwt.Services.AuthServices.Contracts;
using Jennifer.SharedKernel;
using Microsoft.AspNetCore.Http;

namespace Jennifer.Jwt.Application.Auth.Commands.Password;

public sealed record PasswordForgotVerifyCommand(string Email, string Code) : ICommand<IResult>;

public class PasswordForgotVerifyCommandHandler(
    IVerifyCodeService verifyCodeService   
    ):ICommandHandler<PasswordForgotVerifyCommand, IResult>
{
    public async Task<Result<IResult>> HandleAsync(PasswordForgotVerifyCommand command, CancellationToken cancellationToken)
    {
        var result = await verifyCodeService.HandleAsync(new VerifyCodeRequest(command.Email, command.Code, ENUM_EMAIL_VERIFICATION_TYPE.PASSWORD_FORGOT), cancellationToken);
        if(result.Status != ENUM_VERITY_RESULT_STATUS.EMAIL_CONFIRM) return TypedResults.BadRequest(result.Message);
        return TypedResults.Ok();
    }
}

public class PasswordForgotVerifyCommandValidator : AbstractValidator<PasswordForgotVerifyCommand>
{
    public PasswordForgotVerifyCommandValidator()
    {
        RuleFor(m => m.Email).NotEmpty().EmailAddress();
        RuleFor(m => m.Code).NotEmpty().MinimumLength(6).MaximumLength(6);
    }
}