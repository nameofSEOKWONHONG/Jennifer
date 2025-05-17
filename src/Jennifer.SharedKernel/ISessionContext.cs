namespace Jennifer.SharedKernel;

public interface ISessionContext
{
    string Email { get; set; }
    string UserId { get; set; }
    bool IsAuthenticated { get; }
    IApplicationDbContext ApplicationDbContext { get; set; }
}