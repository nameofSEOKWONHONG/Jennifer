namespace Jennifer.Tenant.Models;

public interface IAuditable
{
    DateTimeOffset CreatedOn { get; set; }
    DateTimeOffset? ModifiedOn { get; set; }
}