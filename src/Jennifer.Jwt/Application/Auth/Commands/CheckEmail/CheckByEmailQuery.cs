using eXtensionSharp;
using FluentValidation;
using Jennifer.Jwt.Abstractions.Messaging;
using Jennifer.Jwt.Models;
using Jennifer.SharedKernel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Jennifer.Jwt.Application.Auth.Commands.CheckEmail;

public sealed record CheckByEmailQuery(string Email): IQuery<IResult>;

public class CheckByEmailQueryHandler(UserManager<User> userManager): IQueryHandler<CheckByEmailQuery, IResult>
{
    public async Task<Result<IResult>> HandleAsync(CheckByEmailQuery query, CancellationToken cancellationToken)
    {
        var exists = await userManager.FindByEmailAsync(query.Email);
        if(exists.xIsNotEmpty()) return TypedResults.BadRequest("User already exists");
        
        return TypedResults.Ok();
    }
}

public class CheckByEmailQueryValidator : AbstractValidator<CheckByEmailQuery>
{
    public CheckByEmailQueryValidator()
    {
        RuleFor(m => m.Email).NotEmpty().EmailAddress();
    }
}