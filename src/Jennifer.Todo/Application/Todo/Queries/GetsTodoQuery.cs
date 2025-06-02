using System.Diagnostics;
using eXtensionSharp;
using Jennifer.Account.Grpc;
using Jennifer.Infrastructure.Database;
using Jennifer.Infrastructure.Session;
using Jennifer.SharedKernel;
using Jennifer.Todo.Application.Todo.Contracts;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Jennifer.Todo.Application.Todo.Queries;

public sealed record GetsTodoQuery(string Description, int PageNo, int PageSize): IQuery<PaginatedResult<TodoItemDto>>;
public sealed class GetsTodoQueryHandler(
    JenniferDbContext dbContext,
    ISessionContext session,
    AccountService.AccountServiceClient client,
    ITodoQueryFilter filter,
    ILogger<GetsTodoQueryHandler> logger): IQueryHandler<GetsTodoQuery, PaginatedResult<TodoItemDto>>
{
    public async ValueTask<PaginatedResult<TodoItemDto>> Handle(GetsTodoQuery query, CancellationToken cancellationToken)
    {
        var user = await session.User.Current.GetAsync();
        
        //TODO: GRPC EXAMPLE
        //PERFORMANCE: DEBUG=60ms, RELEASE=10ms
        var sw = Stopwatch.StartNew();
        var reply = await client.GetUserInfoAsync(new AccountUserRequest() { UserId = user.Id.ToString() });
        sw.Stop();
        
        logger.LogInformation("grpc call time: {time}", sw.ElapsedMilliseconds);
        if (reply.xIsEmpty())
        {
            logger.LogError("user not found");
        }
        else
        {
            logger.LogInformation("user found, id:{id}, email:{email}, username:{username}",
                reply.UserId, reply.Email, reply.UserName);
        }
        //TODO: GRPC EXAMPLE
        
        
        var queryable = dbContext.TodoItems.AsNoTracking().Where(filter.Where(query, user.Id));
        var total = await queryable.CountAsync(cancellationToken: cancellationToken);
        var items = await queryable
            .Select(filter.Selector)
            .Skip((query.PageNo - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToArrayAsync(cancellationToken: cancellationToken);

        return await PaginatedResult<TodoItemDto>.SuccessAsync(total, items, query.PageNo, query.PageSize);
    }
}