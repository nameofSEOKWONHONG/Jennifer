using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Jennifer.Infrastructure.MessageQueues;

public abstract class ProcessorBase<TProcessor>(ILogger<TProcessor> logger,
    IServiceScopeFactory serviceScopeFactory,
    int delayInMilliseconds = 1000) : BackgroundService
{
    protected sealed override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "RunAsync Error");
            }

            await Task.Delay(delayInMilliseconds, stoppingToken);
        }
    }
    
    protected abstract Task RunAsync(CancellationToken cancellationToken);
}