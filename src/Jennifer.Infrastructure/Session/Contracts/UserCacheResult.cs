namespace Jennifer.Infrastructure.Session.Contracts;

public class UserCacheResult
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string ConcurrencyStamp { get; set; }
}