namespace Jennifer.SharedKernel;

public interface IDomainEventEntity
{
    List<IDomainEvent> DomainEvents { get; }
    void ClearDomainEvents();
    void Raise(IDomainEvent domainEvent);
}