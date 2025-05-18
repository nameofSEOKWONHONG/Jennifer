using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using eXtensionSharp;
using Jennifer.Jwt.Abstractions.Messaging;
using Jennifer.Jwt.Models;
using Jennifer.SharedKernel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Jennifer.Jwt.Application.Auth.Commands.SignOut;

public sealed record SignOutCommand(bool dummy):ICommand<IResult>;

public class SignOutCommandHandler(
    UserManager<User> userManager,
    IHttpContextAccessor accessor    
    ): ICommandHandler<SignOutCommand, IResult>
{
    public async Task<Result<IResult>> HandleAsync(SignOutCommand command, CancellationToken cancellationToken)
    {
        var sid = accessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(sid.xIsEmpty()) return TypedResults.BadRequest("Not found");
        
        var user = await userManager.FindByIdAsync(sid);
        if(user is null) return TypedResults.BadRequest("Not found user");
        
        var result = await userManager.RemoveAuthenticationTokenAsync(user, loginProvider:"internal", tokenName:"refreshToken");
        return TypedResults.Ok(result.Succeeded);
    }
}