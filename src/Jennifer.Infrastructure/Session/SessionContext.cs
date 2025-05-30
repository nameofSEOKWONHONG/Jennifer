using Jennifer.Infrastructure.Session.Abstracts;
using Jennifer.Infrastructure.Session.Contracts;
using Jennifer.SharedKernel;
using Microsoft.Extensions.DependencyInjection;

namespace Jennifer.Infrastructure.Session;

public static class SessionContextKeyedServiceName
{
    public const string User = "User";
    public const string Option = "Option";
    public const string UserOption = "UserOption";
}

public class SessionContext : ISessionContext
{
    public IUnifiedContext<UserCacheResult> User { get; }
    public IUnifiedContext<UserOptionCacheResult[]> UserOption { get; }
    public IUnifiedContext<OptionCacheResult[]> Option { get; }
    public IDomainEventPublisher DomainEventPublisher { get; }
    public bool IsAuthenticated => !string.IsNullOrEmpty(User?.Sid);

    public SessionContext(
        [FromKeyedServices(SessionContextKeyedServiceName.User)] IUnifiedContext<UserCacheResult> user,
        [FromKeyedServices(SessionContextKeyedServiceName.UserOption)] IUnifiedContext<UserOptionCacheResult[]> userOption,
        [FromKeyedServices(SessionContextKeyedServiceName.Option)] IUnifiedContext<OptionCacheResult[]> option,
        IDomainEventPublisher domainEventPublisher)
    {
        User = user;
        Option = option;
        UserOption = userOption;
        DomainEventPublisher = domainEventPublisher;
    }
}