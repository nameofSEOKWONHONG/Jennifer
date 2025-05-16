using Jennifer.SharedKernel.Infrastructure;
using Jennifer.Jwt.Models;
using Jennifer.Jwt.Models.EntityConfigurations;
using Jennifer.SharedKernel.Infrastructure.Session;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartEnum.EFCore;

namespace Jennifer.Jwt.Data;

public class JenniferDbContext : IdentityDbContext<User, Role, Guid, 
    UserClaim, 
    UserRole, 
    UserLogin, 
    RoleClaim,
    UserToken>, IDbContext
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

        modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserClaimEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserLoginEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserRoleEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserTokenEntityConfiguration());
        modelBuilder.ApplyConfiguration(new RoleEntityConfiguration());
        modelBuilder.ApplyConfiguration(new RoleClaimEntityConfiguration());
        modelBuilder.ApplyConfiguration(new EmailVerificationCodeEntityConfiguration());
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
    
    public DbSet<EmailVerificationCode> EmailVerificationCodes { get; set; }
}