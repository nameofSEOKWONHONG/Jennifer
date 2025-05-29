using Jennifer.Domain.Account;
using Jennifer.Domain.Common;
using Jennifer.SharedKernel;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartEnum.EFCore;

namespace Jennifer.Infrastructure.Database;

public class JenniferDbContext : IdentityDbContext<User, Role, Guid, 
    UserClaim, 
    UserRole, 
    UserLogin, 
    RoleClaim,
    UserToken>, ITransactionDbContext
{  
    public DbSet<EmailVerificationCode> EmailVerificationCodes { get; set; }
    public DbSet<Option> Options { get; set; }
    public DbSet<Audit> Audits { get; set; }
    public DbSet<KafkaDeadLetter> KafkaDeadLetters { get; set; }
    public DbSet<UserOption> UserOptions { get; set; }

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
        modelBuilder.ApplyConfiguration(new OptionEntityConfiguration());
        modelBuilder.ApplyConfiguration(new AuditEntityConfiguration());
        modelBuilder.ApplyConfiguration(new KafkaDeadLetterConfiguration());
        modelBuilder.ApplyConfiguration(new UserOptionEntityConfiguration());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        foreach (var entry in ChangeTracker.Entries<IAuditable>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedOn = DateTimeOffset.UtcNow;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.ModifiedOn = DateTimeOffset.UtcNow;
            }
        }
        
        var result = await base.SaveChangesAsync(cancellationToken);
        return result;
    }
}