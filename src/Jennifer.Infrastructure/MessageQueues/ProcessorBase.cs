using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Jennifer.Infrastructure.MessageQueues;

public abstract class ProcessorBase<TProcessor> : BackgroundService
{
    protected readonly ILogger<TProcessor> Logger;
    protected readonly IServiceScopeFactory ServiceScopeFactory;
    protected readonly int DelayInMilliseconds;

    public ProcessorBase(ILogger<TProcessor> logger,
        IServiceScopeFactory serviceScopeFactory,
        int delayInMilliseconds = 1000)
    {
        this.Logger = logger;
        this.ServiceScopeFactory = serviceScopeFactory;
        this.DelayInMilliseconds = delayInMilliseconds;
    }
    
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
                this.Logger.LogError(ex, "RunAsync Error");
            }

            await Task.Delay(DelayInMilliseconds, stoppingToken);
        }
    }
    
    protected abstract Task RunAsync(CancellationToken cancellationToken);
}