using eXtensionSharp;
using FluentValidation;
using Jennifer.Domain.Accounts;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.AspNetCore.Identity;

namespace Jennifer.Account.Application.Auth.Commands.CheckEmail;

/// <summary>
/// Handles checking if an email address already exists
/// </summary>
public sealed class CheckByEmailQueryHandler(UserManager<User> userManager): IRequestHandler<CheckByEmailQuery, Result>
{
    /// <summary>
    /// Checks if the given email already exists in the system
    /// </summary>
    /// <param name="request">Query containing email to check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success if email is available, Failure if already exists</returns>
    public async ValueTask<Result> Handle(CheckByEmailQuery request, CancellationToken cancellationToken)
    {
        var exists = await userManager.FindByEmailAsync(request.Email);
        if(exists.xIsNotEmpty())
            return await Result.FailureAsync(new Error(string.Empty, "Email already exists"));

        return await Result.SuccessAsync();
    }
}

/// <summary>
/// Validator for the CheckByEmailQuery
/// </summary>
public sealed class CheckByEmailQueryValidator : AbstractValidator<CheckByEmailQuery>
{
    public CheckByEmailQueryValidator()
    {
        RuleFor(m => m.Email).NotEmpty().EmailAddress();
    }
}