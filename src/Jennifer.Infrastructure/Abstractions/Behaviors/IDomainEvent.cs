using Mediator;

namespace Jennifer.Infrastructure.Abstractions.Behaviors;

public interface IDomainEvent : INotification
{
}

public interface IDomainEventPublisher
{
    void Enqueue(IDomainEvent domainEvent);
    bool IsEmpty();
    Task PublishEnqueuedAsync(CancellationToken cancellationToken = default);
}