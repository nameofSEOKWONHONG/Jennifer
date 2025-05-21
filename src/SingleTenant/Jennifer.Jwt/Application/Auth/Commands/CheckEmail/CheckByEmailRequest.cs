using eXtensionSharp;
using FluentValidation;
using Jennifer.Jwt.Models;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.AspNetCore.Identity;

namespace Jennifer.Jwt.Application.Auth.Commands.CheckEmail;

public sealed record CheckByEmailRequest(string Email): IRequest<Result>;

public sealed class CheckByEmailQueryHandler(UserManager<User> userManager): IRequestHandler<CheckByEmailRequest, Result>
{
    public async ValueTask<Result> Handle(CheckByEmailRequest request, CancellationToken cancellationToken)
    {
        var exists = await userManager.FindByEmailAsync(request.Email);
        if(exists.xIsNotEmpty())
            return Result.Failure(new Error(string.Empty, "Email already exists"));

        return Result.Success();
    }
}

public class CheckByEmailQueryValidator : AbstractValidator<CheckByEmailRequest>
{
    public CheckByEmailQueryValidator()
    {
        RuleFor(m => m.Email).NotEmpty().EmailAddress();
    }
}