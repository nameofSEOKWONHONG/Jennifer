using Jennifer.Account.Behaviors;
using Jennifer.Infrastructure.Abstractions.Behaviors;
using Mediator;

namespace Jennifer.Account.Models;

public interface IEntity
{
    List<IDomainEvent> DomainEvents { get; }
    void Clear();
    void Raise(IDomainEvent domainEvent);
}