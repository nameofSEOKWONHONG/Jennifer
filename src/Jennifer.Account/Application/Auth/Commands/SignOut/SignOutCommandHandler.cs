using System.Security.Claims;
using eXtensionSharp;
using Jennifer.Domain.Account;
using Jennifer.Infrastructure.Session;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Hybrid;

namespace Jennifer.Account.Application.Auth.Commands.SignOut;

internal sealed class SignOutCommandHandler(
    UserManager<User> userManager,
    IHttpContextAccessor accessor,
    HybridCache cache
): ICommandHandler<SignOutCommand, Result>
{
    public async ValueTask<Result> Handle(SignOutCommand command, CancellationToken cancellationToken)
    {
        var sid = accessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (sid.xIsEmpty()) 
            return await Result.FailureAsync("not found sid");
        
        var user = await userManager.FindByIdAsync(sid);
        if(user is null) 
            return await Result.FailureAsync("not found user");
        
        var result = await userManager.RemoveAuthenticationTokenAsync(user, loginProvider:"internal", tokenName:"refreshToken");
        if(!result.Succeeded) return await Result.FailureAsync("not found refreshToken");

        var key = CachingConsts.UserCacheKey(user.Id);
        await cache.RemoveAsync(key, cancellationToken);
        
        return await Result.SuccessAsync();
    }
}