using Jennifer.Domain.Accounts;
using Jennifer.Domain.Common;
using Jennifer.Domain.Todos;
using Jennifer.Infrastructure.Session;
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
    private readonly IUserContext _user;
    private readonly DomainEventDispatcher _dispatcher;
    public DbSet<EmailConfirmCode> EmailVerificationCodes { get; set; }

    #region [common]

    public DbSet<Option> Options { get; set; }
    public DbSet<Audit> Audits { get; set; }
    public DbSet<KafkaDeadLetter> KafkaDeadLetters { get; set; }
    public DbSet<UserOption> UserOptions { get; set; }
    public DbSet<IpBlockLog> IpBlockLogs { get; set; }
    public DbSet<Menu> Menus { get; set; }    

    #endregion

    #region [example - todo]

    public DbSet<TodoItem> TodoItems { get; set; }
    public DbSet<TodoItemShare> TodoItemShares { get; set; }    

    #endregion

    public JenniferDbContext(DbContextOptions<JenniferDbContext> options, IUserContext user, DomainEventDispatcher dispatcher): base(options)
    {
        _user = user;
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
        
        #region [common]
        modelBuilder.ApplyConfiguration(new OptionEntityConfiguration());
        modelBuilder.ApplyConfiguration(new AuditEntityConfiguration());
        modelBuilder.ApplyConfiguration(new KafkaDeadLetterConfiguration());
        modelBuilder.ApplyConfiguration(new UserOptionEntityConfiguration());
        modelBuilder.ApplyConfiguration(new IpBlockLogEntityConfiguration());
        modelBuilder.ApplyConfiguration(new MenuEntityConfiguration());        
        #endregion

        #region [example - todo]

        modelBuilder.ApplyConfiguration(new TodoItemEntityConfiguration());
        modelBuilder.ApplyConfiguration(new TodoItemShareEntityConfiguration());        

        #endregion

    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var currentUser = await _user.Current.GetAsync();
        foreach (var entry in ChangeTracker.Entries<IAuditable>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedOn = DateTimeOffset.UtcNow;
                entry.Entity.CreatedBy = currentUser.Id.ToString();
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.ModifiedOn = DateTimeOffset.UtcNow;
                entry.Entity.ModifiedBy = currentUser.Id.ToString();
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