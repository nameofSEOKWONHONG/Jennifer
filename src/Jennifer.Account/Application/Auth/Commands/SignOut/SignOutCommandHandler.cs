using System.Security.Claims;
using eXtensionSharp;
using Jennifer.Domain.Accounts;
using Jennifer.Infrastructure.Session;
using Jennifer.Infrastructure.Session.Abstracts;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Hybrid;

namespace Jennifer.Account.Application.Auth.Commands.SignOut;

public sealed class SignOutCommandHandler(
    UserManager<User> userManager,
    ISessionContext session
): ICommandHandler<SignOutCommand, Result>
{
    public async ValueTask<Result> Handle(SignOutCommand command, CancellationToken cancellationToken)
    {
        var user = await session.User.GetAsync(); 
        if (user.xIsEmpty()) 
            return await Result.FailureAsync("not found user");
        
        var exists = await userManager.FindByIdAsync(user.Id.ToString());
        var result = await userManager.RemoveAuthenticationTokenAsync(exists, loginProvider:"internal", tokenName:"refreshToken");
        if(!result.Succeeded) return await Result.FailureAsync("not found refreshToken");

        await session.User.ClearAsync();
        
        return await Result.SuccessAsync();
    }
}