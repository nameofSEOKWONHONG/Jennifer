using Mediator;

namespace Jennifer.Jwt.Models;

public interface IEntity
{
    List<INotification> Notifications { get; }
    void ClearNotifications();
    void Raise(INotification notification);
}