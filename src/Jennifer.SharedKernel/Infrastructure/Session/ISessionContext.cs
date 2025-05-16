namespace Jennifer.SharedKernel.Infrastructure.Session;

public interface IDbContext;

public interface ISessionContext
{
    string Email { get; set; }
    string UserId { get; set; }
    bool IsAuthenticated { get; }
    IDbContext DbContext { get; set; }
}