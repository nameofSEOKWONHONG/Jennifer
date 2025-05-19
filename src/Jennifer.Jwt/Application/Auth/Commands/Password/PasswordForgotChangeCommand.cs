using eXtensionSharp;
using Jennifer.Jwt.Abstractions.Messaging;
using Jennifer.Jwt.Application.Auth.Services.Abstracts;
using Jennifer.Jwt.Application.Auth.Services.Contracts;
using Jennifer.Jwt.Models;
using Jennifer.Jwt.Models.Contracts;
using Jennifer.SharedKernel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Jennifer.Jwt.Application.Auth.Commands.Password;

public sealed record PasswordForgotChangeCommand(string Email, string Code, string Password): ICommand<IResult>;

public class PasswordForgotChangeCommandHandler(UserManager<User> userManager,
    IPasswordHasher<User> passwordHasher,
    IVerifyCodeConfirmService verifyCodeConfirmService): ICommandHandler<PasswordForgotChangeCommand, IResult>
{
    public async Task<Result<IResult>> HandleAsync(PasswordForgotChangeCommand command, CancellationToken cancellationToken)
    {
        var verified = await verifyCodeConfirmService.HandleAsync(new VerifyCodeRequest(command.Email, command.Code, ENUM_EMAIL_VERIFICATION_TYPE.PASSWORD_FORGOT), cancellationToken);
        if(verified.Status != ENUM_VERITY_RESULT_STATUS.EMAIL_CONFIRM)
            return TypedResults.BadRequest(verified.Message);
        
        var user = await userManager.FindByEmailAsync(command.Email);
        if(user.xIsEmpty()) return TypedResults.BadRequest("Not found");
        
        var hashPassword = passwordHasher.HashPassword(user, command.Password);
        user.PasswordHash = hashPassword;
        await userManager.UpdateAsync(user);
        return TypedResults.Ok();
    }
}