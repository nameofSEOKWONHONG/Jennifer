namespace Jennifer.SharedKernel.Infrastructure.Session;

public interface ISessionContextInitializer
{
    void Initialize(string userId, string email, IDbContext dbContext); 
}

public class SessionContext : ISessionContext, ISessionContextInitializer
{
    public string Email { get; set; }
    public string UserId { get; set; }
    public bool IsAuthenticated => !string.IsNullOrEmpty(UserId);
    public IDbContext DbContext { get; set; }

    public SessionContext()
    {
    }

    public void Initialize(string userId, string email, IDbContext dbContext)
    {
        this.UserId = userId;
        this.Email = email;
        this.DbContext = dbContext;
    }
}

