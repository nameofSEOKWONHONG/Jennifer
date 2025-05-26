using Jennifer.Account.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Account.Data;

public class JenniferReadOnlyDbContext: IdentityDbContext<User, Role, Guid, 
    UserClaim, 
    UserRole, 
    UserLogin, 
    RoleClaim,
    UserToken>
{       
    public DbSet<EmailVerificationCode> EmailVerificationCodes { get; set; }
    public DbSet<Configuration> Configurations { get; set; }

    public JenniferReadOnlyDbContext(DbContextOptions<JenniferReadOnlyDbContext> options): base(options)
    {
        
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
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
    
    public override int SaveChanges()
    {
        throw new InvalidOperationException("Read-only context: SaveChanges is not allowed.");
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Read-only context: SaveChangesAsync is not allowed.");
    }
}