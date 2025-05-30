using System.Security.Claims;
using eXtensionSharp;
using Jennifer.Domain.Accounts;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Jennifer.Account.Application.Auth.Commands.Password;

public sealed class PasswordChangeCommandHandler(
    UserManager<User> userManager, 
    IHttpContextAccessor accessor     
): ICommandHandler<PasswordChangeCommand, Result<bool>>
{
    public async ValueTask<Result<bool>> Handle(PasswordChangeCommand command, CancellationToken cancellationToken)
    {
        var userId = accessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var user = await userManager.FindByIdAsync(userId);
        if(user.xIsEmpty()) 
            return await Result<bool>.FailureAsync("Not found");
        
        var result = await userManager.ChangePasswordAsync(user, command.OldPassword, command.NewPassword);
        if (!result.Succeeded) 
            return await Result<bool>.FailureAsync(result.Errors.Select(m => m.Description).First());

        return await Result<bool>.SuccessAsync(true);
    }
}
