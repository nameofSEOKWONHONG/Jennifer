using eXtensionSharp;
using Jennifer.Infrastructure.Data;
using Jennifer.Jwt.Models;
using Jennifer.Jwt.Session.Abstracts;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartEnum.EFCore;

namespace Jennifer.Jwt.Data;

public class JenniferDbContext : IdentityDbContext<User, Role, Guid, 
    UserClaim, 
    UserRole, 
    UserLogin, 
    RoleClaim,
    UserToken>, IApplicationDbContext
{
    private readonly IUserContext _userContext;
    private readonly IMediator _mediator;

    public JenniferDbContext(DbContextOptions<JenniferDbContext> options,
        IUserContext userContext,
        IMediator mediator): base(options)
    {
        _userContext = userContext;
        _mediator = mediator;
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.ConfigureSmartEnum();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserClaimEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserLoginEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserRoleEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserTokenEntityConfiguration());
        modelBuilder.ApplyConfiguration(new RoleEntityConfiguration());
        modelBuilder.ApplyConfiguration(new RoleClaimEntityConfiguration());
        modelBuilder.ApplyConfiguration(new EmailVerificationCodeEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ConfigurationEntityConfiguration());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        foreach (var entry in ChangeTracker.Entries<IAuditable>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedBy = _userContext.UserId ?? "SELF";
                entry.Entity.CreatedOn = DateTimeOffset.UtcNow;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.ModifiedBy = _userContext.UserId ?? "SELF";
                entry.Entity.ModifiedOn = DateTimeOffset.UtcNow;
            }
        }
        
        var result = await base.SaveChangesAsync(cancellationToken);

        await PublishDomainEventsAsync();

        return result;
    }
    
    public DbSet<EmailVerificationCode> EmailVerificationCodes { get; set; }
    public DbSet<Configuration> Configurations { get; set; }
    
    private async Task PublishDomainEventsAsync()
    {
        var notifications = ChangeTracker
            .Entries<IEntity>()
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                List<INotification> entityNotifications = entity.Notifications;

                entity.ClearNotifications();

                return entityNotifications;
            })
            .ToList();

        foreach (var notification in notifications)
        {
            await _mediator.Publish(notification);
        }
    }
}