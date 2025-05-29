using eXtensionSharp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Jennifer.Infrastructure.MessageQueues;

/// <summary>
/// Provides a base implementation for a batch processing service that operates asynchronously in the background.
/// The service processes items in parallel from a source database context to a target database context, using
/// specified degrees of parallelism.
/// </summary>
/// <typeparam name="TService">The type of the service deriving from this base class.</typeparam>
/// <typeparam name="TDbContext">The type of the source database context to produce items from.</typeparam>
/// <typeparam name="TItem">The type of the items being processed.</typeparam>
/// <remarks>
/// Derive this class to implement a custom background service for batch processing. The derived class must provide
/// specific implementations for the methods to produce and consume items.
/// </remarks>
public abstract class BatchServiceBase<TService, TDbContext, TItem>(
    ILogger<TService> logger,
    IServiceScopeFactory serviceScopeFactory,
    int maxDegreeOfParallelism = 4,
    int delayInMilliseconds = 1000)
    : BackgroundService
    where TService : class
    where TDbContext : DbContext
{
    protected readonly ILogger<TService> logger = logger;

    protected sealed override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var scope = serviceScopeFactory.CreateAsyncScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();

                var items = await ProduceAsync(dbContext, stoppingToken);
                if (items.xIsEmpty())
                {
                    await Task.Delay(delayInMilliseconds, stoppingToken);
                    continue;
                }
                
                var parallelOptions = new ParallelOptions
                {
                    CancellationToken = stoppingToken,
                    MaxDegreeOfParallelism = maxDegreeOfParallelism
                };
                await Parallel.ForEachAsync(items, parallelOptions, async (item, token) =>
                {
                    try
                    {
                        await using var innerScope = serviceScopeFactory.CreateAsyncScope();
                        var scopedDb = innerScope.ServiceProvider.GetRequiredService<TDbContext>();
                        await ConsumeAsync(item, scopedDb, token);
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, "Consume Failed for item: {Item}", item);

                    }
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Consumer Outer Error");
            }

            await Task.Delay(delayInMilliseconds, stoppingToken);
        }
    }

    protected abstract Task<IEnumerable<TItem>> ProduceAsync(TDbContext dbContext, CancellationToken cancellationToken);
    protected abstract Task ConsumeAsync(TItem item, TDbContext dbContext, CancellationToken cancellationToken);
}

/// <summary>
/// Represents an abstract base class for implementing a batch processing service that runs asynchronously
/// as a background task. The service facilitates the production and consumption of items between two
/// database contexts using specified levels of parallelism and an optional delay.
/// </summary>
/// <typeparam name="TService">The type of the service implementing this base class.</typeparam>
/// <typeparam name="TDbContextFrom">The type of the source database context from which items are produced.</typeparam>
/// <typeparam name="TDbContextTo">The type of the target database context where items are consumed.</typeparam>
/// <typeparam name="TItem">The type of the items being processed.</typeparam>
/// <param name="logger">The logger instance to log service execution details.</param>
/// <param name="serviceScopeFactory">The service scope factory to create scopes for resolving dependencies.</param>
/// <param name="maxDegreeOfParallelism">
/// The maximum number of operations that can run concurrently during the processing of items. Default value is 4.
/// </param>
/// <param name="delayInMilliseconds">
/// The delay time, in milliseconds, between each processing iteration to control the processing rate. Default value is 1000.
/// </param>
/// <remarks>
/// This class is intended to be derived to create services that require background batch processing logic.
/// Derived classes must override the <c>ProduceAsync</c> and <c>ConsumeAsync</c> methods to customize
/// the production of items from the source context and the consumption of those items in the target context.
/// </remarks>
public abstract class BatchServiceBase<TService, TDbContextFrom, TDbContextTo, TItem>(
    ILogger<TService> logger,
    IServiceScopeFactory serviceScopeFactory,
    int maxDegreeOfParallelism = 4,
    int delayInMilliseconds = 1000) : BackgroundService
    where TService : class
    where TDbContextFrom : DbContext
{
    protected readonly ILogger<TService> logger = logger;

    protected sealed override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var scope = serviceScopeFactory.CreateAsyncScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<TDbContextFrom>();

                var items = await ProduceAsync(dbContext, stoppingToken);
                if (items.xIsEmpty())
                {
                    await Task.Delay(delayInMilliseconds, stoppingToken);
                    continue;
                }
                
                var parallelOptions = new ParallelOptions
                {
                    CancellationToken = stoppingToken,
                    MaxDegreeOfParallelism = maxDegreeOfParallelism
                };
                await Parallel.ForEachAsync(items, parallelOptions, async (item, token) =>
                {
                    try
                    {
                        await using var innerScope = serviceScopeFactory.CreateAsyncScope();
                        var scopedDb = innerScope.ServiceProvider.GetRequiredService<TDbContextTo>();
                        await ConsumeAsync(item, scopedDb, token);
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, "Consume Failed for item: {Item}", item);

                    }
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Consumer Outer Error");
            }

            await Task.Delay(delayInMilliseconds, stoppingToken);
        }
    }

    protected abstract Task<IEnumerable<TItem>> ProduceAsync(TDbContextFrom dbContext, CancellationToken cancellationToken);
    protected abstract Task ConsumeAsync(TItem item, TDbContextTo dbContext, CancellationToken cancellationToken);
}