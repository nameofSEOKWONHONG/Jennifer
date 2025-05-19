using Jennifer.SharedKernel;

namespace Jennifer.Jwt.Session.Abstracts;

public interface ISessionContextInitializer
{
    Task Initialize(IApplicationDbContext applicationDbContext);
}
