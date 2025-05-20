using System.Data.Entity;
using eXtensionSharp;
using Jennifer.Infrastructure.Abstractions.Messaging;
using Jennifer.Jwt.Application.Auth.Contracts;
using Jennifer.Jwt.Data;
using Jennifer.Jwt.Session.Abstracts;
using Jennifer.SharedKernel;
using Microsoft.Identity.Client;

namespace Jennifer.Jwt.Application.Users.Commands;

public class GetUserQueryHandler(ISessionContext context,
    IUserQueryFilter queryFilter): IQueryHandler<GetUserQuery, UserDto>
{
    public async Task<Result<UserDto>> HandleAsync(GetUserQuery query, CancellationToken cancellationToken) =>
        await context.ApplicationDbContext
            .xAs<JenniferDbContext>()
            .Users
            .AsNoTracking()
            .Where(queryFilter.Where(query))
            .Select(queryFilter.Selector)
            .FirstOrDefaultAsync(cancellationToken);
}