using Jennifer.Infrastructure.Data;

namespace Jennifer.Account.Session.Abstracts;

public interface ISessionContext
{
    IUserContext UserContext { get; }   
    bool IsAuthenticated { get; }
    IApplicationDbContext ApplicationDbContext { get; }
}