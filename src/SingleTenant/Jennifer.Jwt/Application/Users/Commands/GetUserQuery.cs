using eXtensionSharp;
using FluentValidation;
using Jennifer.Infrastructure.Abstractions.Messaging;
using Jennifer.Jwt.Application.Auth.Contracts;
using Jennifer.Jwt.Data;
using Jennifer.Jwt.Session;
using Jennifer.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Jwt.Application.Users.Commands;

public sealed record GetUserQuery(Guid UserId): IQuery<UserDto>;

public class GetUserQueryHandler(ISessionContext context): IQueryHandler<GetUserQuery, UserDto>
{
    public async Task<Result<UserDto>> HandleAsync(GetUserQuery query, CancellationToken cancellationToken) =>
        await context.ApplicationDbContext
            .xAs<JenniferDbContext>()
            .Users
            .AsNoTracking()
            .Where(m => m.Id == query.UserId)
            .Select(m => new UserDto
            {
                Id = m.Id,
                Email = m.Email,
                PhoneNumber = m.PhoneNumber,
                UserName = m.UserName
            })
            .FirstAsync(cancellationToken);
}

public class GetUserQueryValidator : AbstractValidator<GetUserQuery>
{
    public GetUserQueryValidator()
    {
        RuleFor(m => m.UserId).NotEmpty();
    }
}