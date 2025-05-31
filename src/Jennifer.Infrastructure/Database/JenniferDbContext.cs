using Jennifer.Domain.Accounts;
using Jennifer.Domain.Common;
using Jennifer.Domain.Todos;
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
    private readonly DomainEventDispatcher _dispatcher;
    public DbSet<EmailConfirmCode> EmailVerificationCodes { get; set; }
    public DbSet<Option> Options { get; set; }
    public DbSet<Audit> Audits { get; set; }
    public DbSet<KafkaDeadLetter> KafkaDeadLetters { get; set; }
    public DbSet<UserOption> UserOptions { get; set; }
    
    public DbSet<TodoItem> TodoItems { get; set; }

    public JenniferDbContext(DbContextOptions<JenniferDbContext> options, DomainEventDispatcher dispatcher): base(options)
    {
        _dispatcher = dispatcher;
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
        
        modelBuilder.ApplyConfiguration(new TodoItemEntityConfiguration());
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
        
        // 트랜잭션 이전 이벤트 수집
        var domainEntities = ChangeTracker.Entries<IHasDomainEvents>()
            .Where(e =>
                e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted &&
                e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        var result = await base.SaveChangesAsync(cancellationToken);

        // 트랜잭션 이후 퍼블리시
        if (_dispatcher is not null)
            await _dispatcher.DispatchAsync(domainEntities);
        
        return result;
    }
}