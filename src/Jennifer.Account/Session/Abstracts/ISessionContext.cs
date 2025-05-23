using Jennifer.Infrastructure.Data;

namespace Jennifer.Account.Session.Abstracts;

public interface ISessionContext
{
    IUserContext User { get; }
    bool IsAuthenticated { get; }
}