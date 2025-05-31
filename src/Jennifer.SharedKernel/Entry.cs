using Mediator;

namespace Jennifer.SharedKernel;

public abstract class Entry : IHasDomainEvents, IAuditable
{
    public List<INotification> DomainEvents { get; } = new();
    public void ClearDomainEvents()
    {
        DomainEvents.Clear();   
    }

    public DateTimeOffset CreatedOn { get; set; }
    public string CreatedBy { get; set; }
    public DateTimeOffset? ModifiedOn { get; set; }
    public string ModifiedBy { get; set; }
}