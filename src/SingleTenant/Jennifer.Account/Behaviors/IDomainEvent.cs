using Mediator;

namespace Jennifer.Account.Behaviors;

public interface IDomainEvent : INotification
{
}

public interface IDomainEventPublisher
{
    void Enqueue(IDomainEvent domainEvent);
    Task PublishEnqueuedAsync(CancellationToken cancellationToken = default);
}