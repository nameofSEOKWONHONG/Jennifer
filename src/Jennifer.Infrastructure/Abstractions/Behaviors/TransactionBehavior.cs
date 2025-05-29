using Jennifer.Domain.Common;
using Mediator;
using Microsoft.Extensions.Logging;

namespace Jennifer.Infrastructure.Abstractions.Behaviors;

public sealed class TransactionBehavior<TRequest, TResponse>(ITransactionDbContext dbContext, ILogger<TransactionBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
{
    public async ValueTask<TResponse> Handle(TRequest message, MessageHandlerDelegate<TRequest, TResponse> next, CancellationToken cancellationToken)
    {
        // 조건: 트랜잭션 명시된 커맨드만 처리
        // if (message is not ITransactionCommand && message is not ITransactionCommand<TResponse>)
        //     return await next(message, cancellationToken);

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