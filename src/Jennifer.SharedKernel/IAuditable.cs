namespace Jennifer.SharedKernel;

public interface IAuditable
{
    DateTimeOffset CreatedOn { get; set; }
    public string CreatedBy { get; set; }
    DateTimeOffset? ModifiedOn { get; set; }
    public string ModifiedBy { get; set; }
}