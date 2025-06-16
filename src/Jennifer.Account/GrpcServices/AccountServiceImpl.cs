using eXtensionSharp;
using Grpc.Core;
using Jennifer.Account.Grpc;
using Jennifer.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Account.GrpcServices;

public class AccountServiceImpl(
    JenniferReadOnlyDbContext dbContext
    ) : Jennifer.Account.Grpc.AccountService.AccountServiceBase
{
    public override async Task<UserReply> GetUserInfo(AccountUserRequest request, ServerCallContext context)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(m => m.Id == Guid.Parse(request.UserId));
        if (user.xIsEmpty()) return null;

        return new UserReply
        {
            UserId = user.Id.ToString(),
            Email = user.Email,
            UserName = user.UserName
        };
    }
}