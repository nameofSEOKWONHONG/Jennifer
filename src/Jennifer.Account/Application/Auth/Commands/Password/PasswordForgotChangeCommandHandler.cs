using eXtensionSharp;
using Jennifer.Account.Application.Auth.Services.Abstracts;
using Jennifer.Domain.Accounts;
using Jennifer.Domain.Accounts.Contracts;
using Jennifer.Infrastructure.Abstractions.ServiceCore;
using Jennifer.SharedKernel;
using Jennifer.SharedKernel.Account.Auth;
using Mediator;
using Microsoft.AspNetCore.Identity;

namespace Jennifer.Account.Application.Auth.Commands.Password;

public sealed class PasswordForgotChangeCommandHandler(
    UserManager<User> userManager,
    IPasswordHasher<User> passwordHasher,
    IServiceExecutionBuilderFactory factory): ICommandHandler<PasswordForgotChangeCommand, Result>
{
    public async ValueTask<Result> Handle(PasswordForgotChangeCommand command, CancellationToken cancellationToken)
    {
        var builder = factory.Create();
        Result verified = null;
        await builder.Register<IEmailConfirmService, EmailConfirmRequest, Result>()
            .Request(new EmailConfirmRequest(command.Email, command.Code, ENUM_EMAIL_VERIFY_TYPE.PASSWORD_FORGOT.Name))
            .Handle(r => verified = r)
            .ExecuteAsync(cancellationToken);
        
        if(!verified.IsSuccess)
            return await Result.FailureAsync(verified.Message);
        
        var user = await userManager.FindByEmailAsync(command.Email);
        if(user.xIsEmpty()) return await Result.FailureAsync("Not found");
        
        var hashPassword = passwordHasher.HashPassword(user, command.Password);
        user.PasswordHash = hashPassword;
        await userManager.UpdateAsync(user);
        return await Result.SuccessAsync();
    }
}