﻿using Jennifer.Domain.Accounts;
using Jennifer.Infrastructure.Database;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Account.Application.Users.Commands;

public sealed class CreateUserClaimCommandHandler(JenniferDbContext dbContext): ICommandHandler<CreateUserClaimCommand, Result>
{
    public async ValueTask<Result> Handle(CreateUserClaimCommand command, CancellationToken cancellationToken)
    {
        await dbContext.UserClaims.Where(m => m.UserId == command.UserId)
            .ExecuteDeleteAsync(cancellationToken: cancellationToken);
        
        var list = new List<UserClaim>();
        foreach (var createUserClaimRequest in command.requests)
        {
            list.Add(new UserClaim()
            {
                UserId = command.UserId,
                ClaimType = createUserClaimRequest.ClaimType,
                ClaimValue = createUserClaimRequest.ClaimValue,
            });
        }
        await dbContext.UserClaims.AddRangeAsync(list, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return await Result.SuccessAsync();
    }
}
