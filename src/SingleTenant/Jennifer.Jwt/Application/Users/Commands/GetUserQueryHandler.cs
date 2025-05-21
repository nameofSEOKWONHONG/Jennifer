using eXtensionSharp;
using Jennifer.Infrastructure.Abstractions.Messaging;
using Jennifer.Jwt.Application.Auth.Contracts;
using Jennifer.Jwt.Data;
using Jennifer.Jwt.Session.Abstracts;
using Jennifer.SharedKernel;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Jwt.Application.Users.Commands;

public class GetUserQueryHandler(ISessionContext context,
    JenniferDbContext dbContext,
    IUserQueryFilter queryFilter): IQueryHandler<GetUserQuery, UserDto>
{
    public async Task<Result<UserDto>> HandleAsync(GetUserQuery query, CancellationToken cancellationToken) =>
        await dbContext
            .xAs<JenniferDbContext>()
            .Users
            .AsNoTracking()
            .AsExpandable()
            .Where(queryFilter.Where(query))
            .Select(queryFilter.Selector)
            .FirstOrDefaultAsync(cancellationToken);
}