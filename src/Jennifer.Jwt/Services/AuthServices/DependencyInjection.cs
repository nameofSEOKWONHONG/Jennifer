using FluentValidation;
using Jennifer.Jwt.Abstractions.Behaviors;
using Jennifer.Jwt.Abstractions.Messaging;
using Jennifer.Jwt.DomainEvents;
using Jennifer.Jwt.Services.AuthServices.Abstracts;
using Jennifer.Jwt.Services.AuthServices.Implements;
using Jennifer.SharedKernel;
using Microsoft.Extensions.DependencyInjection;

namespace Jennifer.Jwt.Services.AuthServices;

internal static class DependencyInjection
{
    public static void AddAuthService(this IServiceCollection services)
    {
        services.AddScoped<ICheckEmailService, CheckEmailService>();
        services.AddScoped<IPasswordChangeService, PasswordChangeService>();
        services.AddScoped<IPasswordForgotChangeService, PasswordForgotChangeService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<ISignInService, SignInService>();
        services.AddScoped<ISignOutService, SignOutService>();
        services.AddScoped<ISignUpService, SignUpService>();
        services.AddScoped<ISignUpAdminService, SignUpAdminService>();
        services.AddScoped<IVerifyCodeByEmailSendService, VerifyCodeByEmailSendService>();
        services.AddScoped<IVerifyCodeService, VerifyCodeService>();
        
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


    }
}