using FluentValidation;
using Jennifer.Infrastructure.Abstractions.Behaviors;
using Jennifer.Infrastructure.Abstractions.DomainEvents;
using Jennifer.Infrastructure.Abstractions.Messaging;
using Jennifer.Jwt.Application.Auth.Contracts;
using Jennifer.Jwt.Application.Auth.Services.Abstracts;
using Jennifer.Jwt.Application.Auth.Services.Implements;
using Jennifer.Jwt.Services;
using Jennifer.SharedKernel;
using Microsoft.Extensions.DependencyInjection;

namespace Jennifer.Jwt.Application.Auth;

internal static class DependencyInjection
{
    public static void AddAuthService(this IServiceCollection services)
    {
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IVerifyCodeSendEmailService, VerifyCodeSendEmailService>();
        services.AddScoped<IVerifyCodeConfirmService, VerifyCodeConfirmService>();
        services.AddScoped<IConfigurationAddService, ConfigurationAddService>();
        services.AddScoped<IExternalOAuthService, ExternalOAuthService>();
        services.AddValidatorsFromAssemblyContaining<UserDtoValidator>(); // 자동 검증 필터 추가
        
        services.Scan(scan => scan.FromAssembliesOf(typeof(DependencyInjection))
            .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.Decorate(typeof(ICommandHandler<,>), typeof(ValidationDecorator.CommandHandler<,>));
        services.Decorate(typeof(ICommandHandler<>), typeof(ValidationDecorator.CommandBaseHandler<>));

        services.Decorate(typeof(IQueryHandler<,>), typeof(LoggingDecorator.QueryHandler<,>));
        services.Decorate(typeof(ICommandHandler<,>), typeof(LoggingDecorator.CommandHandler<,>));
        services.Decorate(typeof(ICommandHandler<>), typeof(LoggingDecorator.CommandBaseHandler<>));

        services.Scan(scan => scan.FromAssembliesOf(typeof(DependencyInjection))
            .AddClasses(classes => classes.AssignableTo(typeof(IDomainEventHandler<>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);
        
        services.AddTransient<IDomainEventsDispatcher, DomainEventsDispatcher>();
        services.AddScoped
            <ISender, Sender>();
    }
}