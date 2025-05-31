using Mediator;

namespace Jennifer.SharedKernel;

public interface IHasDomainEvents
{
    List<INotification> DomainEvents { get; }
    void ClearDomainEvents();
}


public interface IAuditable
{
    DateTimeOffset CreatedOn { get; set; }
    public string CreatedBy { get; set; }
    DateTimeOffset? ModifiedOn { get; set; }
    public string ModifiedBy { get; set; }
}