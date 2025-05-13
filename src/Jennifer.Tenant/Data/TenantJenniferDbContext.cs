using Jennifer.Tenant.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartEnum.EFCore;
using Role = Jennifer.Tenant.Models.Role;

namespace Jennifer.Tenant.Data;

public class TenantJenniferDbContext: IdentityDbContext<User, Role, Guid, 
    UserClaim, 
    UserRole, 
    UserLogin, 
    RoleClaim,
    UserToken>
{
    public TenantJenniferDbContext(DbContextOptions<TenantJenniferDbContext> options): base(options)
    {
        
    }
    
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.ConfigureSmartEnum();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new Tenant.Models.Tenant.TenantEntityConfiguration());
        modelBuilder.ApplyConfiguration(new User.UserEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserClaim.UserClaimEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserLogin.UserLoginEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserRole.UserRoleEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserToken.UserTokenEntityConfiguration());
        modelBuilder.ApplyConfiguration(new Role.RoleEntityConfiguration());
        modelBuilder.ApplyConfiguration(new RoleClaim.RoleClaimEntityConfiguration());
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        foreach (var entry in ChangeTracker.Entries<IAuditable>())
        {
            if (entry.State == EntityState.Added)
                entry.Entity.CreatedOn = DateTimeOffset.UtcNow;
            if (entry.State == EntityState.Modified)
                entry.Entity.ModifiedOn = DateTimeOffset.UtcNow;
        }
        return base.SaveChangesAsync(cancellationToken);
    }
    
    public DbSet<Models.Tenant> Tenants { get; set; }
}