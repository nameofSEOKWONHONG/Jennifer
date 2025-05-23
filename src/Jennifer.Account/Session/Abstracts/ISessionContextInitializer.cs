using Jennifer.Infrastructure.Data;

namespace Jennifer.Account.Session.Abstracts;

public interface ISessionContextInitializer
{
    Task Initialize(IApplicationDbContext applicationDbContext);
}
