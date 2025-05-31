using Jennifer.SharedKernel;
using Mediator;
using Microsoft.Extensions.Logging;

namespace Jennifer.Infrastructure.Abstractions.DomainEvents;


public class DomainEventPublisher : INotificationPublisher
{
    private readonly ILogger<DomainEventPublisher> _logger;

    public DomainEventPublisher(ILogger<DomainEventPublisher> logger)
    {
        _logger = logger;
    }

    public async ValueTask Publish<TNotification>(NotificationHandlers<TNotification> handlers, TNotification notification,
        CancellationToken cancellationToken) where TNotification : INotification
    {
        _logger.LogInformation("🔥 DomainEventPublisher invoked: {Event}", typeof(TNotification).Name);

        foreach (var notificationHandler in handlers)
        {
            await notificationHandler.Handle(notification, cancellationToken);
        }
    }
}