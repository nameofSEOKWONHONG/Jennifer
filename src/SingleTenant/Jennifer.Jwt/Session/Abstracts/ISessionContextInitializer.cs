using Jennifer.Infrastructure.Data;

namespace Jennifer.Jwt.Session.Abstracts;

public interface ISessionContextInitializer
{
    Task Initialize();
}
