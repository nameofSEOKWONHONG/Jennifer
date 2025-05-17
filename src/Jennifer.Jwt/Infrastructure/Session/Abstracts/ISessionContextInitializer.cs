using Jennifer.SharedKernel;

namespace Jennifer.Jwt.Infrastructure.Session.Abstracts;

public interface ISessionContextInitializer
{
    void Initialize(string userId, string email, IApplicationDbContext applicationDbContext);
}
