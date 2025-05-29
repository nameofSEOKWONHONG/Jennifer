namespace Jennifer.SharedKernel;

public interface IDomainEventPublisher
{
    void Enqueue(IDomainEvent domainEvent);
    bool IsEmpty();
    Task PublishEnqueuedAsync(CancellationToken cancellationToken = default);
}
