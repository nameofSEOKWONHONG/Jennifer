using System.Security.Claims;
using eXtensionSharp;
using Jennifer.Jwt.Models;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Jennifer.Jwt.Application.Auth.Commands.SignOut;

public sealed record SignOutCommand(bool dummy):ICommand<Result>;

public class SignOutCommandHandler(
    UserManager<User> userManager,
    IHttpContextAccessor accessor    
    ): ICommandHandler<SignOutCommand, Result>
{
    public async ValueTask<Result> Handle(SignOutCommand command, CancellationToken cancellationToken)
    {
        var sid = accessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (sid.xIsEmpty()) 
            return Result.Failure("not found sid");
        
        var user = await userManager.FindByIdAsync(sid);
        if(user is null) 
            return Result.Failure("not found user");
        
        var result = await userManager.RemoveAuthenticationTokenAsync(user, loginProvider:"internal", tokenName:"refreshToken");
        if(!result.Succeeded) return Result.Failure("not found refreshToken");
        
        return Result.Success();
    }
}