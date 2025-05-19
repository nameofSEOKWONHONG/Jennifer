using Jennifer.Infrastructure.Abstractions.DomainEvents;
using Jennifer.SharedKernel;
using Jennifer.Tenant.Models;
using Jennifer.Tenant.Session;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartEnum.EFCore;

namespace Jennifer.Tenant.Data;

public class JenniferTenantDbContext : IdentityDbContext<User, Role, Guid, 
    UserClaim, 
    UserRole, 
    UserLogin, 
    RoleClaim,
    UserToken>, IApplicationDbContext
{
    private readonly IDomainEventsDispatcher _domainEventDispatcher;
    private readonly IUserContext _userContext;

    public JenniferTenantDbContext(DbContextOptions<JenniferTenantDbContext> options,
        IUserContext userContext,
        IDomainEventsDispatcher domainEventDispatcher): base(options)
    {
        _userContext = userContext;
        _domainEventDispatcher = domainEventDispatcher;
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.ConfigureSmartEnum();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new TenantEntityConfiguration());
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
        var domainEvents = ChangeTracker
            .Entries<IEntity>()
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                List<IDomainEvent> domainEvents = entity.DomainEvents;

                entity.ClearDomainEvents();

                return domainEvents;
            })
            .ToList();

        await _domainEventDispatcher.DispatchAsync(domainEvents);
    }
}