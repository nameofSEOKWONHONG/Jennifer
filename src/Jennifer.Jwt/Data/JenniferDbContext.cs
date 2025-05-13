using Jennifer.Core.Infrastructure;
using Jennifer.Jwt.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartEnum.EFCore;

namespace Jennifer.Jwt.Data;

public class JenniferDbContext : IdentityDbContext<User, Role, Guid, 
    UserClaim, 
    UserRole, 
    UserLogin, 
    RoleClaim,
    UserToken>
{
    public JenniferDbContext(DbContextOptions<JenniferDbContext> options): base(options)
    {
        
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.ConfigureSmartEnum();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

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
}