using System.Data;
using Jennifer.Domain.Accounts;
using Jennifer.Infrastructure.Database;
using Jennifer.Infrastructure.Extensions;
using Jennifer.Infrastructure.MessageQueues;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Jennifer.Account.Application.Users;

/*
 * 두개를 비교할 경우 압도적으로 DeleteUserProcessor가 성능 측면에서 유리합니다.
 * 하지만 왜 DeleteUserBatchProcessor같은 시나리오를 만들까요?
 * 이건 조회화 처리를 완전히 분리하고자 하는 의도입니다.
 * 성능에서 분명 분리하지만 개별 처리의 확실한 보장이 필요할 경우 DeleteUserBatchProcessor같은 시나리오를 사용할 수 있습니다.
 */

/// <summary>
/// The DeleteUserProcessor is responsible for handling the deletion of user records from the database
/// based on specific conditions, such as being marked for deletion and meeting date constraints.
/// It extends the ProcessorBase class and provides an implementation for executing the delete operation
/// in a scoped and transactional context.
/// </summary>
public sealed class DeleteUserProcessor(
    ILogger<DeleteUserBatchProcessor> logger,
    IServiceScopeFactory serviceScopeFactory) : ProcessorBase<DeleteUserBatchProcessor>(logger, serviceScopeFactory)
{
    protected override async Task RunAsync(CancellationToken cancellationToken)
    {
        using var scope = ServiceScopeFactory.CreateScope();
        var executor = scope.ServiceProvider.GetRequiredService<IDapperExecutor>();
        var transaction = await executor.BeginTransactionAsync(IsolationLevel.ReadUncommitted);
        
        try
        {
            var users = await executor.QueryAsync<User>("SELECT TOP 100 Id FROM Users WHERE IsDelete = true AND ModifiedOn >= @date", 
                new { date = DateTimeOffset.UtcNow.AddDays(-30) });
            
            var ids = users.Select(m => m.Id);
            await executor.ExecuteAsync("DELETE FROM Users WHERE Id in @ids", new { ids });
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(cancellationToken);
            Logger.LogError(e, "Delete user search fail"); 
        }
    }
}

/// <summary>
/// The DeleteUserBatchProcessor is a background service designed to handle batch deletions of user records from the database.
/// It extends BatchProcessorBase, leveraging its parallel processing and scoped execution capabilities to manage batched operations.
/// This processor identifies users marked for deletion, ensures they meet specified conditions, such as a grace period,
/// and executes the deletion process transactionally.
/// </summary>
public sealed class DeleteUserBatchProcessor(
    ILogger<DeleteUserBatchProcessor> logger,
    IServiceScopeFactory serviceScopeFactory) : BatchProcessorBase<DeleteUserBatchProcessor, JenniferDbContext, User>(logger, serviceScopeFactory)
{
    protected override async Task<IEnumerable<User>> ProduceAsync(JenniferDbContext dbContext, CancellationToken cancellationToken)
    {
        return await dbContext.Users
            .Where(m => m.IsDelete)
            .Where(m => m.ModifiedOn.Value.AddDays(30) >= DateTimeOffset.UtcNow)
            .Take(100)
            .ToArrayAsync(cancellationToken: cancellationToken);
    }

    protected override async Task ConsumeAsync(User item, JenniferDbContext dbContext, CancellationToken cancellationToken)
    {
        dbContext.Users.Remove(item);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}