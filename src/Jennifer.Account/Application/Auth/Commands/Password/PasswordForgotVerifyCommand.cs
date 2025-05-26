using FluentValidation;
using Jennifer.Account.Application.Auth.Contracts;
using Jennifer.Account.Application.Auth.Services.Abstracts;
using Jennifer.Account.Models.Contracts;
using Jennifer.Infrastructure.Abstractions.ServiceCore;
using Jennifer.SharedKernel;
using Mediator;

namespace Jennifer.Account.Application.Auth.Commands.Password;

public sealed record PasswordForgotVerifyCommand(string Email, string Code) : ICommand<Result>;

internal sealed class PasswordForgotVerifyCommandHandler(
    IServiceExecutionBuilderFactory factory,
    IVerifyCodeConfirmService verifyCodeConfirmService   
    ):ICommandHandler<PasswordForgotVerifyCommand, Result>
{
    public async ValueTask<Result> Handle(PasswordForgotVerifyCommand command, CancellationToken cancellationToken)
    {
        Result result = null;
        var builder = factory.Create();
        await builder.Register<IVerifyCodeConfirmService, VerifyCodeRequest, Result>()
            .Request(new VerifyCodeRequest(command.Email, command.Code, ENUM_EMAIL_VERIFICATION_TYPE.PASSWORD_FORGOT))
            .Handle(r => result = r)
            .ExecuteAsync(cancellationToken);
        
        if(!result.IsSuccess) return Result.Failure(result.Message);
        return Result.Success();
    }
}

internal class PasswordForgotVerifyCommandValidator : AbstractValidator<PasswordForgotVerifyCommand>
{
    public PasswordForgotVerifyCommandValidator()
    {
        RuleFor(m => m.Email).NotEmpty().EmailAddress();
        RuleFor(m => m.Code).NotEmpty().MinimumLength(6).MaximumLength(6);
    }
}