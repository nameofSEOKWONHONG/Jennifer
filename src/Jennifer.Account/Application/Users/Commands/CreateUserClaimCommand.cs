using Jennifer.Domain.Account;
using Jennifer.Domain.Database;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Account.Application.Users.Commands;

public sealed record CreateUserClaimRequest(string ClaimType, string ClaimValue);
public sealed record CreateUserClaimCommand(Guid UserId, CreateUserClaimRequest[] requests) : ICommand<Result>;

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
        
        return Result.Success();
    }
}