using eXtensionSharp;
using FluentValidation;
using Jennifer.Domain.Account;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.AspNetCore.Identity;

namespace Jennifer.Account.Application.Auth.Commands.CheckEmail;

public sealed record CheckByEmailRequest(string Email);

public sealed record CheckByEmailQuery(string Email): IRequest<Result>;

internal sealed class CheckByEmailQueryHandler(UserManager<User> userManager): IRequestHandler<CheckByEmailQuery, Result>
{
    public async ValueTask<Result> Handle(CheckByEmailQuery request, CancellationToken cancellationToken)
    {
        var exists = await userManager.FindByEmailAsync(request.Email);
        if(exists.xIsNotEmpty())
            return await Result.FailureAsync(new Error(string.Empty, "Email already exists"));

        return await Result.SuccessAsync();
    }
}

internal sealed class CheckByEmailQueryValidator : AbstractValidator<CheckByEmailQuery>
{
    public CheckByEmailQueryValidator()
    {
        RuleFor(m => m.Email).NotEmpty().EmailAddress();
    }
}