using System.Reflection;
using Jennifer.Infrastructure.Database;
using Mediator;
using Microsoft.Extensions.Logging;

namespace Jennifer.Infrastructure.Abstractions.Behaviors;

public sealed class TransactionBehavior<TRequest, TResponse>(ITransactionDbContext dbContext, ILogger<TransactionBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
{
    public async ValueTask<TResponse> Handle(TRequest message, MessageHandlerDelegate<TRequest, TResponse> next, CancellationToken cancellationToken)
    {
        var attr = typeof(TRequest).GetCustomAttribute<UseTransactionAttribute>();
        if (attr is null)
        {
            // 트랜잭션 없이 그대로 다음 핸들러 실행
            return await next(message, cancellationToken);
        }

        logger.LogDebug("Begin Transaction for {Command}", typeof(TRequest).Name);
        
        if(dbContext.Database.CurrentTransaction != null)
            return await next(message, cancellationToken);

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var response = await next(message, cancellationToken);
            
            await transaction.CommitAsync(cancellationToken);

            logger.LogDebug("Committed Transaction for {Command}", typeof(TRequest).Name);
            
            return response;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            logger.LogError(ex, "Transaction failed for {Command}", typeof(TRequest).Name);
            throw;
        }
    }
}