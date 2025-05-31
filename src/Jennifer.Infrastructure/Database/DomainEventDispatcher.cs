using Jennifer.SharedKernel;
using Mediator;

namespace Jennifer.Infrastructure.Database;

public sealed class DomainEventDispatcher(IPublisher publisher)
{
    public async Task DispatchAsync(IEnumerable<IHasDomainEvents> entities)
    {
        foreach (var entity in entities)
        {
            foreach (var domainEvent in entity.DomainEvents)
            {
                await publisher.Publish(domainEvent);
            }
            entity.ClearDomainEvents();
        }
    }
}
