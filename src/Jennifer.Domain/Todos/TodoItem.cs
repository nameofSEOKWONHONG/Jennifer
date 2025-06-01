using Jennifer.Domain.Converters;
using Jennifer.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jennifer.Domain.Todos;

public class TodoItem : Entry
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Description { get; set; }
    public DateTime? DueDate { get; set; }
    public List<string> Labels { get; set; } = [];
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public Priority Priority { get; set; }
    
    public virtual ICollection<TodoItemShare> TodoItemShares { get; set; }

    public static TodoItem Create(Guid userId, string description, DateTime? dueDate, List<string> labels,  bool isCompleted, DateTime? completedAt, Priority priority)
    {
        var newItem = new TodoItem()
        {
            UserId = userId,
            Description = description,
            DueDate = dueDate,
            Labels = labels,
            IsCompleted = isCompleted,
            CompletedAt = completedAt,
            Priority = priority,
            CreatedOn = DateTimeOffset.UtcNow,
            CreatedBy = userId.ToString(),
        };

        return newItem;
    }
}

public class TodoItemEntityConfiguration : IEntityTypeConfiguration<TodoItem>
{
    public void Configure(EntityTypeBuilder<TodoItem> builder)
    {
        builder.ToTable("TodoItems", "todos");
        builder.HasKey(m => new {m.Id, m.UserId});
        builder.Property(m => m.Id)
            .ValueGeneratedOnAdd()
            .HasValueGenerator<GuidV7ValueGenerator>();
        builder.Property(m => m.Description)
            .HasMaxLength(4000);
        
        builder.Property(m => m.CreatedBy)
            .HasMaxLength(36)
            .IsRequired();
        builder.Property(m => m.CreatedOn)
            .HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");
        builder.Property(m => m.ModifiedBy)
            .HasMaxLength(36);
        
        builder.HasMany(m => m.TodoItemShares)
            .WithOne(m => m.TodoItem)
            .HasForeignKey(m => new {m.TodoItemId, m.UserId})
            .OnDelete(DeleteBehavior.Cascade);
    }
}