using eXtensionSharp;
using FluentValidation;
using Jennifer.Jwt.Application.Auth.Contracts;
using Jennifer.Jwt.Application.Auth.Services.Abstracts;
using Jennifer.Jwt.Data;
using Jennifer.Jwt.Models.Contracts;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Jwt.Application.Auth.Commands.SignUp;

public sealed record SignUpVerifyRequest(Guid UserId, string Code);

public sealed record SignUpVerifyCommand(Guid UserId, string Code):ICommand<Result>;

public class SignUpVerifyCommandHandler(JenniferDbContext dbContext,
    IVerifyCodeConfirmService verifyCodeConfirmService): ICommandHandler<SignUpVerifyCommand, Result>
{
    public async ValueTask<Result> Handle(SignUpVerifyCommand command, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(m => m.Id == command.UserId, cancellationToken: cancellationToken);
        if (user.xIsEmpty())
            return Result.Failure("Not Found User");
        
        var verified = await verifyCodeConfirmService.HandleAsync(new VerifyCodeRequest(user.Email, command.Code, ENUM_EMAIL_VERIFICATION_TYPE.SIGN_UP_BEFORE), cancellationToken);
        if(verified.Status != ENUM_VERITY_RESULT_STATUS.EMAIL_CONFIRM) 
            return Result.Failure(verified.Message);

        user.EmailConfirmed = true;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

public class SignUpVerifyCommandValidator : AbstractValidator<SignUpVerifyCommand>
{
    public SignUpVerifyCommandValidator()
    {
        RuleFor(m => m.UserId).NotEmpty();
        RuleFor(m => m.Code).NotEmpty();
    }
}