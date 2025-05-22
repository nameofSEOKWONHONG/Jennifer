using eXtensionSharp;
using Jennifer.Account.Data;
using Jennifer.Infrastructure.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Jennifer.Account.Behaviors;

public sealed class TransactionBehavior<TRequest, TResponse>(IApplicationDbContext dbContext, ILogger<TransactionBehavior<TRequest, TResponse>> logger,
    IDomainEventPublisher domainEventPublisher) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
{
    public async ValueTask<TResponse> Handle(TRequest message, MessageHandlerDelegate<TRequest, TResponse> next, CancellationToken cancellationToken)
    {
        // 조건: 트랜잭션 명시된 커맨드만 처리
        if (message is not ITransactionCommand && message is not ITransactionCommand<TResponse>)
            return await next(message, cancellationToken);

        logger.LogDebug("Begin Transaction for {Command}", typeof(TRequest).Name);

        await using var transaction = await dbContext.xAs<DbContext>().Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var response = await next(message, cancellationToken);
            
            await transaction.CommitAsync(cancellationToken);

            logger.LogDebug("Committed Transaction for {Command}", typeof(TRequest).Name);
            
            // Commit 후에만 Notification 발행
            await domainEventPublisher.PublishEnqueuedAsync(cancellationToken);

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