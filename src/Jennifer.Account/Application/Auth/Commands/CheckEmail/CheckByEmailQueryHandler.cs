using eXtensionSharp;
using FluentValidation;
using Jennifer.Domain.Accounts;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.AspNetCore.Identity;

namespace Jennifer.Account.Application.Auth.Commands.CheckEmail;


public sealed class CheckByEmailQueryHandler(UserManager<User> userManager): IRequestHandler<CheckByEmailQuery, Result>
{
    public async ValueTask<Result> Handle(CheckByEmailQuery request, CancellationToken cancellationToken)
    {
        var exists = await userManager.FindByEmailAsync(request.Email);
        if(exists.xIsNotEmpty())
            return await Result.FailureAsync(new Error(string.Empty, "Email already exists"));

        return await Result.SuccessAsync();
    }
}

public sealed class CheckByEmailQueryValidator : AbstractValidator<CheckByEmailQuery>
{
    public CheckByEmailQueryValidator()
    {
        RuleFor(m => m.Email).NotEmpty().EmailAddress();
    }
}