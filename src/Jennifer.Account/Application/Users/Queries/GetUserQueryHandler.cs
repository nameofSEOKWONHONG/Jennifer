﻿using Jennifer.Infrastructure.Database;
using Jennifer.SharedKernel;
using Jennifer.SharedKernel.Account.Auth;
using LinqKit;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Account.Application.Users.Queries;

public sealed class GetUserQueryHandler(
    IUserQueryFilter queryFilter,
    JenniferDbContext dbContext): IQueryHandler<GetUserQuery, Result<UserDto>>
{
    public async ValueTask<Result<UserDto>> Handle(GetUserQuery query, CancellationToken cancellationToken)
    {
        var result = await dbContext
            .Users
            .Include(m => m.UserClaims)
            .AsNoTracking()
            .AsExpandable()
            .Where(queryFilter.Where(query))
            .Select(queryFilter.Selector)
            .FirstOrDefaultAsync(cancellationToken);
        
        return await Result<UserDto>.SuccessAsync(result);
    }
}