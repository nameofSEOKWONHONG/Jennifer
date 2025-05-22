using Mediator;

namespace Jennifer.Account.Models;

public interface IEntity
{
    List<INotification> Notifications { get; }
    void ClearNotifications();
    void Raise(INotification notification);
}