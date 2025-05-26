using Jennifer.Infrastructure.Abstractions.Behaviors;

namespace Jennifer.Account.Models;

public interface IEntity
{
    List<IDomainEvent> DomainEvents { get; }
    void Clear();
    void Raise(IDomainEvent domainEvent);
}