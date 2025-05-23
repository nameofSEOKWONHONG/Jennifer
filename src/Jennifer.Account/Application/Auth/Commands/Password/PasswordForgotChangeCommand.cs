using eXtensionSharp;
using Jennifer.Account.Application.Auth.Contracts;
using Jennifer.Account.Application.Auth.Services.Abstracts;
using Jennifer.Account.Models;
using Jennifer.Account.Models.Contracts;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Jennifer.Account.Application.Auth.Commands.Password;

public sealed record PasswordForgotChangeCommand(string Email, string Code, string Password): ICommand<Result>;

internal sealed class PasswordForgotChangeCommandHandler(UserManager<User> userManager,
    IPasswordHasher<User> passwordHasher,
    IVerifyCodeConfirmService verifyCodeConfirmService): ICommandHandler<PasswordForgotChangeCommand, Result>
{
    public async ValueTask<Result> Handle(PasswordForgotChangeCommand command, CancellationToken cancellationToken)
    {
        var verified = await verifyCodeConfirmService.HandleAsync(new VerifyCodeRequest(command.Email, command.Code, ENUM_EMAIL_VERIFICATION_TYPE.PASSWORD_FORGOT), cancellationToken);
        if(verified.Status != ENUM_VERITY_RESULT_STATUS.EMAIL_CONFIRM)
            return Result.Failure(verified.Message);
        
        var user = await userManager.FindByEmailAsync(command.Email);
        if(user.xIsEmpty()) return Result.Failure("Not found");
        
        var hashPassword = passwordHasher.HashPassword(user, command.Password);
        user.PasswordHash = hashPassword;
        await userManager.UpdateAsync(user);
        return Result.Success();
    }
}