using Jennifer.Infrastructure.Session.Abstracts;
using Jennifer.Infrastructure.Session.Contracts;
using Jennifer.SharedKernel;
using Microsoft.Extensions.DependencyInjection;

namespace Jennifer.Infrastructure.Session.Implements;

public static class SessionContextKeyedServiceName
{
    public const string User = "User";
    public const string Option = "Option";
    public const string UserOption = "UserOption";
}

public class SessionContext : ISessionContext
{
    public IUnifiedContext<UserFetchResult> User { get; }
    public IUnifiedContext<UserOptionFetchResult[]> UserOption { get; }
    public IUnifiedContext<OptionFetchResult[]> Option { get; }
    public IDomainEventPublisher DomainEventPublisher { get; }
    public bool IsAuthenticated => !string.IsNullOrEmpty(User?.Sid);

    public SessionContext(
        [FromKeyedServices(SessionContextKeyedServiceName.User)] IUnifiedContext<UserFetchResult> user,
        [FromKeyedServices(SessionContextKeyedServiceName.UserOption)] IUnifiedContext<UserOptionFetchResult[]> userOption,
        [FromKeyedServices(SessionContextKeyedServiceName.Option)] IUnifiedContext<OptionFetchResult[]> option,
        IDomainEventPublisher domainEventPublisher)
    {
        User = user;
        Option = option;
        UserOption = userOption;
        DomainEventPublisher = domainEventPublisher;
    }
}