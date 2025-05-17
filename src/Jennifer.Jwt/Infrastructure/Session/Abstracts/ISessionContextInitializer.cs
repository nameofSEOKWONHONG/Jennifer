using Jennifer.SharedKernel;

namespace Jennifer.Jwt.Infrastructure.Session.Abstracts;

public interface ISessionContextInitializer
{
    Task Initialize(IApplicationDbContext applicationDbContext);
}
