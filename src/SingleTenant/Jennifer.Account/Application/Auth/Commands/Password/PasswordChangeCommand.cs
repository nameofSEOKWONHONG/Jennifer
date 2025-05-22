using System.Security.Claims;
using eXtensionSharp;
using FluentValidation;
using Jennifer.Account.Models;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Jennifer.Account.Application.Auth.Commands.Password;

//로그인 된 상태에서 암호 변경을 의미.

public sealed record PasswordChangeCommand(string OldPassword, string NewPassword):ICommand<Result<bool>>;

public class PasswordChangeCommandHandler(
    UserManager<User> userManager, 
    IHttpContextAccessor accessor     
    ): ICommandHandler<PasswordChangeCommand, Result<bool>>
{
    public async ValueTask<Result<bool>> Handle(PasswordChangeCommand command, CancellationToken cancellationToken)
    {
        var userId = accessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var user = await userManager.FindByIdAsync(userId);
        if(user.xIsEmpty()) 
            return Result<bool>.Failure("Not found");
        
        var result = await userManager.ChangePasswordAsync(user, command.OldPassword, command.NewPassword);
        if (!result.Succeeded) 
            return Result<bool>.Failure(result.Errors.Select(m => m.Description).First());

        return Result<bool>.Success(true);
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