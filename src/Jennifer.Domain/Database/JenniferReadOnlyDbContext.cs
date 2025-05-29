using Jennifer.Domain.Account;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Domain.Database;

/// <summary>
/// Represents a read-only version of a database context for managing application identity and account-related data.
/// Inherits from IdentityDbContext to include functionality related to users, roles, and claims.
/// </summary>
/// <remarks>
/// This context enforces a read-only mode by disabling modification operations through overridden SaveChanges
/// and SaveChangesAsync methods. It is configured to prevent query tracking to optimize performance for read-only scenarios.
/// </remarks>
public class JenniferReadOnlyDbContext: IdentityDbContext<User, Role, Guid, 
    UserClaim, 
    UserRole, 
    UserLogin, 
    RoleClaim,
    UserToken>
{       
    public DbSet<EmailVerificationCode> EmailVerificationCodes { get; set; }
    public DbSet<Option> Configurations { get; set; }

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
        modelBuilder.ApplyConfiguration(new OptionEntityConfiguration());
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