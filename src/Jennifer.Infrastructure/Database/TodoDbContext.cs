using eXtensionSharp;
using Jennifer.Domain.Todos;
using Jennifer.Infrastructure.Session;
using Jennifer.SharedKernel;
using Microsoft.EntityFrameworkCore;
using SmartEnum.EFCore;

namespace Jennifer.Infrastructure.Database;

public class TodoDbContext: DbContext
{
    private readonly IUserContext _user;
    private readonly DomainEventDispatcher _dispatcher;

    public TodoDbContext(DbContextOptions<TodoDbContext> options, IUserContext user, DomainEventDispatcher dispatcher): base(options)
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
        
        modelBuilder.ApplyConfiguration(new TodoUserEntityConfiguration());
        modelBuilder.ApplyConfiguration(new TodoItemEntityConfiguration());
        modelBuilder.ApplyConfiguration(new TodoItemShareEntityConfiguration());
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var currentUser = await _user.Current.GetAsync();
        foreach (var entry in ChangeTracker.Entries<IAuditable>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedOn = DateTimeOffset.UtcNow;
                entry.Entity.CreatedBy = currentUser.xIsEmpty() ? "SYSTEM" : currentUser.Id.ToString();
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.ModifiedOn = DateTimeOffset.UtcNow;
                entry.Entity.ModifiedBy = currentUser.xIsEmpty() ? "SYSTEM" : currentUser.Id.ToString();
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

    public DbSet<TodoUser> TodoUsers { get; set; }
    public DbSet<TodoItem> TodoItems { get; set; }
    public DbSet<TodoItemShare> TodoItemShares { get; set; }   
}