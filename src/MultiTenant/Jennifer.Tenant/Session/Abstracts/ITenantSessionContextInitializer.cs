using Jennifer.SharedKernel;

namespace Jennifer.Tenant.Session.Abstracts;

public interface ITenantSessionContextInitializer
{
    Task Initialize(IApplicationDbContext applicationDbContext);
}
