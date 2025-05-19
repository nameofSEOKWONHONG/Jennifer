using eXtensionSharp;
using FluentValidation;
using Jennifer.Infrastructure.Abstractions.Messaging;
using Jennifer.Jwt.Models;
using Jennifer.SharedKernel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Jennifer.Jwt.Application.Auth.Commands.CheckEmail;

public sealed record CheckByEmailQuery(string Email): IQuery<bool>;

public class CheckByEmailQueryHandler(UserManager<User> userManager): IQueryHandler<CheckByEmailQuery, bool>
{
    public async Task<Result<bool>> HandleAsync(CheckByEmailQuery query, CancellationToken cancellationToken)
    {
        var exists = await userManager.FindByEmailAsync(query.Email);
        if(exists.xIsNotEmpty())
            return Result<bool>.Failure(Error.Conflict(string.Empty, "User already exists"));

        return true;
    }
}

public class CheckByEmailQueryValidator : AbstractValidator<CheckByEmailQuery>
{
    public CheckByEmailQueryValidator()
    {
        RuleFor(m => m.Email).NotEmpty().EmailAddress();
    }
}