using Jennifer.SharedKernel;
using Mediator;

namespace Jennifer.Infrastructure.Abstractions.DomainEvents;


public class DomainEventPublisher : IDomainEventPublisher
{
    private readonly List<IDomainEvent> _events = new();
    private readonly IPublisher _publisher;

    public DomainEventPublisher(IPublisher publisher)
    {
        _publisher = publisher;
    }

    public void Enqueue(IDomainEvent domainEvent)
    {
        _events.Add(domainEvent);
    }
    
    public bool IsEmpty() => _events.Count == 0;

    public async Task PublishEnqueuedAsync(CancellationToken cancellationToken = default)
    {
        foreach (var e in _events)
        {
            await _publisher.Publish(e, cancellationToken);
        }
        _events.Clear();
    }
}