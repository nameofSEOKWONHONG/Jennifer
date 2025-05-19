using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using eXtensionSharp;
using Jennifer.Infrastructure.Abstractions.Messaging;
using Jennifer.Jwt.Models;
using Jennifer.SharedKernel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Jennifer.Jwt.Application.Auth.Commands.SignOut;

public sealed record SignOutCommand(bool dummy):ICommand<bool>;

public class SignOutCommandHandler(
    UserManager<User> userManager,
    IHttpContextAccessor accessor    
    ): ICommandHandler<SignOutCommand, bool>
{
    public async Task<Result<bool>> HandleAsync(SignOutCommand command, CancellationToken cancellationToken)
    {
        var sid = accessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (sid.xIsEmpty()) 
            return Result<bool>.Failure(Error.NotFound(string.Empty, "Not found"));
        
        var user = await userManager.FindByIdAsync(sid);
        if(user is null) 
            return Result<bool>.Failure(Error.NotFound(string.Empty, "Not found"));
        
        var result = await userManager.RemoveAuthenticationTokenAsync(user, loginProvider:"internal", tokenName:"refreshToken");
        return result.Succeeded;
    }
}