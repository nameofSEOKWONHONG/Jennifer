using Confluent.Kafka;
using eXtensionSharp;
using eXtensionSharp.Mongo;
using Jennifer.Domain.Common;
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
public abstract class KafkaConsumerProcessorBase<TService>(
    ILogger<TService> logger,
    IConsumer<string, string> consumer,
    IServiceScopeFactory serviceScopeFactory,
    IJMongoFactory mongoFactory,
    string topicName) : BackgroundService
    where TService : class
{
    protected sealed override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        consumer.Subscribe(topicName);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var consumeResult = consumer.Consume(TimeSpan.FromMilliseconds(100));
                if (consumeResult.xIsNotEmpty())
                {
                    await using var scope = serviceScopeFactory.CreateAsyncScope();

                    try
                    {
                        await ConsumeAsync(scope.ServiceProvider, consumeResult, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        var collection = mongoFactory.Create<DeadLetterDocument>();
                        await collection.InsertOneAsync(new DeadLetterDocument()
                        {
                            Topic = consumeResult.Topic,
                            Partition = consumeResult.Partition,
                            Offset = consumeResult.Offset,
                            Key = consumeResult.Message.Key,
                            Value = consumeResult.Message.Value,
                            ErrorMessage = ex.Message
                        }, cancellationToken: stoppingToken);
                    
                        logger.LogError(ex, "Kafka Consumer Inner Error");
                    }
                }
                
                if (consumeResult.xIsNotEmpty())
                {
                    logger.LogInformation("Consumed message '{Message}' at: '{Offset}'.",
                        consumeResult.Message.Value, consumeResult.Offset);
                    consumer.Commit(consumeResult);    
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Kafka Consumer Outer Error");
            }

            await Task.Delay(500, stoppingToken);
        }
    }

    protected abstract Task ConsumeAsync(IServiceProvider service, ConsumeResult<string, string> consumeResult, CancellationToken cancellationToken);
}