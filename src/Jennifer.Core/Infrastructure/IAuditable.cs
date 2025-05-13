namespace Jennifer.Core.Infrastructure;

public interface IAuditable
{
    DateTimeOffset CreatedOn { get; set; }
    DateTimeOffset? ModifiedOn { get; set; }
}