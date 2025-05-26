using eXtensionSharp;
using FluentValidation;
using Jennifer.Account.Application.Auth.Contracts;
using Jennifer.Account.Application.Auth.Services.Abstracts;
using Jennifer.Account.Data;
using Jennifer.Account.Models.Contracts;
using Jennifer.Infrastructure.Abstractions.ServiceCore;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Account.Application.Auth.Commands.SignUp;

public sealed record SignUpVerifyRequest(Guid UserId, string Code);

public sealed record SignUpVerifyCommand(Guid UserId, string Code):ICommand<Result>;

internal sealed class SignUpVerifyCommandHandler(
    JenniferDbContext dbContext,
    IServiceExecutionBuilderFactory factory): ICommandHandler<SignUpVerifyCommand, Result>
{
    public async ValueTask<Result> Handle(SignUpVerifyCommand command, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FirstAsync(m => m.Id == command.UserId, cancellationToken: cancellationToken);

        Result verified = null;
        var builder = factory.Create();
        await builder.Register<IVerifyCodeConfirmService, VerifyCodeRequest, Result>()
            .Request(new VerifyCodeRequest(user.Email, command.Code, ENUM_EMAIL_VERIFICATION_TYPE.SIGN_UP_BEFORE))
            .Handle(r => verified = r)
            .ExecuteAsync(cancellationToken);
                
        if(!verified.IsSuccess) 
            return Result.Failure(verified.Message);

        user.EmailConfirmed = true;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

internal sealed class SignUpVerifyCommandValidator : AbstractValidator<SignUpVerifyCommand>
{
    public SignUpVerifyCommandValidator()
    {
        RuleFor(m => m.UserId).NotEmpty();
        RuleFor(m => m.Code).NotEmpty();
    }
}