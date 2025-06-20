using eXtensionSharp;
using Grpc.Core;
using Jennifer.Domain.Accounts.Contracts;
using Jennifer.Domain.Todos;
using Jennifer.Infrastructure.Database;
using Jennifer.Todo.Grpc;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Todo.GrpcServices;

public sealed class TodoServiceImpl(
    TodoDbContext dbContext
    )
: Jennifer.Todo.Grpc.TodoService.TodoServiceBase
{
    public override async Task<SyncUserInfoResponse> SyncUserInfo(UserData request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.UserId, out var userId))
            return new SyncUserInfoResponse()
            {
                Success = false
            };
        
        var exists = await dbContext.TodoUsers.FirstOrDefaultAsync(m => m.UserId == userId);
        if (exists.xIsEmpty())
        {
            var todoUser = TodoUser.Create(userId, request.Email, request.UserName, ENUM_USER_TYPE.FromValue(request.Type));
            await dbContext.TodoUsers.AddAsync(todoUser);
            await dbContext.SaveChangesAsync();
        }

        return new SyncUserInfoResponse() { Success = true };
    }
}