using Confluent.Kafka;
using Jennifer.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Jennifer.Infrastructure.MessageQueues;

/// <summary>
/// Represents a base class for consuming messages from a Kafka topic.
/// This service subscribes to a specified Kafka topic, processes messages,
/// and handles errors by storing problematic messages in a dead letter table.
/// </summary>
/// <typeparam name="TService">
/// The type of the service that implements this consumer functionality.
/// </typeparam>
/// <typeparam name="TDbContext">
/// The type of the database context to perform data operations. The context import must be registered in KafkaDeadLetter.
/// </typeparam>
public abstract class KafkaConsumerServiceBase<TService, TDbContext>(
    ILogger<TService> logger,
    IConsumer<string, string> consumer,
    IServiceScopeFactory serviceScopeFactory,
    string topicName)
    : BackgroundService
    where TService : class
    where TDbContext : DbContext
{
    protected readonly ILogger<TService> logger = logger;

    protected sealed override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        consumer.Subscribe(topicName);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var consumeResult = consumer.Consume(stoppingToken);
                await using var scope = serviceScopeFactory.CreateAsyncScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();

                try
                {
                    await ConsumeAsync(dbContext, consumeResult, stoppingToken);
                }
                catch (Exception ex)
                {
                    await dbContext.Set<KafkaDeadLetter>()
                        .AddAsync(new KafkaDeadLetter()
                        {
                            Topic = consumeResult.Topic,
                            Partition = consumeResult.Partition,
                            Offset = consumeResult.Offset,
                            Key = consumeResult.Message.Key,
                            Value = consumeResult.Message.Value,
                            ErrorMessage = ex.Message
                        }, stoppingToken);
                    await dbContext.SaveChangesAsync(stoppingToken);
                    
                    logger.LogError(ex, "Kafka Consumer Inner Error");
                }

                consumer.Commit(consumeResult);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Kafka Consumer Outer Error");
            }

            await Task.Delay(500, stoppingToken);
        }
    }

    protected abstract Task ConsumeAsync(TDbContext dbContext, ConsumeResult<string, string> consumeResult, CancellationToken cancellationToken);
}