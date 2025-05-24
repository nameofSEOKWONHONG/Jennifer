using Jennifer.Account.Behaviors;
using Jennifer.Infrastructure.Abstractions.Behaviors;
using Mediator;

namespace Jennifer.Account.Models;

public abstract class Entity : IEntity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public List<IDomainEvent> DomainEvents => [.. _domainEvents];

    public void Clear()
    {
        _domainEvents.Clear();
    }

    public void Raise(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}