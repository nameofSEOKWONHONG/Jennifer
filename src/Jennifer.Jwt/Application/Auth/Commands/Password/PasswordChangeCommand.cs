using System.Security.Claims;
using eXtensionSharp;
using FluentValidation;
using Jennifer.Jwt.Abstractions.Messaging;
using Jennifer.Jwt.Models;
using Jennifer.SharedKernel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Jennifer.Jwt.Application.Auth.Commands.Password;

//로그인 된 상태에서 암호 변경을 의미.

public sealed record PasswordChangeCommand(string OldPassword, string NewPassword):ICommand<IResult>;

public class PasswordChangeCommandHandler(
    UserManager<User> userManager, 
    IHttpContextAccessor accessor     
    ): ICommandHandler<PasswordChangeCommand, IResult>
{
    public async Task<Result<IResult>> HandleAsync(PasswordChangeCommand command, CancellationToken cancellationToken)
    {
        var userId = accessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var user = await userManager.FindByIdAsync(userId);
        if(user.xIsEmpty()) return TypedResults.BadRequest("Not found");
        
        var result = await userManager.ChangePasswordAsync(user, command.OldPassword, command.NewPassword);
        if (!result.Succeeded) return TypedResults.BadRequest(result.Errors.Select(m => m.Description).First());
        
        return TypedResults.Ok(result.Succeeded);
    }
}

public class PasswordCommandValidator : AbstractValidator<PasswordChangeCommand>
{
    public PasswordCommandValidator()
    {
        RuleFor(m => m.OldPassword).NotEmpty().MinimumLength(8);
        RuleFor(m => m.NewPassword).NotEmpty().MinimumLength(8);
    }
}