using Mediator;

namespace Jennifer.Account.Models;

public abstract class Entity : IEntity
{
    private readonly List<INotification> _domainEvents = [];

    public List<INotification> Notifications => [.. _domainEvents];

    public void ClearNotifications()
    {
        _domainEvents.Clear();
    }

    public void Raise(INotification domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}