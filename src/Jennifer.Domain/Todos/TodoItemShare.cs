using Jennifer.Domain.Converters;
using Jennifer.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jennifer.Domain.Todos;

public class TodoItemShare : Entry
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ShareUserId { get; set; }
    
    public Guid TodoItemId { get; set; }
    public TodoItem TodoItem { get; set; }

    public static TodoItemShare Create(Guid userId, Guid shareUserId, Guid todoItemId)
    {
        var newItem = new TodoItemShare()
        {
            UserId = userId,
            ShareUserId = shareUserId,
            TodoItemId = todoItemId,
            CreatedOn = DateTimeOffset.UtcNow,
            CreatedBy = userId.ToString(),
        };
        newItem.DomainEvents.Add(new TodoItemShareEvent(todoItemId, shareUserId));
        return newItem;
    }
}

public class TodoItemShareEntityConfiguration: IEntityTypeConfiguration<TodoItemShare>
{
    public void Configure(EntityTypeBuilder<TodoItemShare> builder)
    {
        builder.ToTable("TodoItemShares", JenniferOptionSingleton.Instance.Options.Schema);
        builder.HasKey(m => new {m.Id, m.UserId, m.ShareUserId});
        builder.Property(m => m.Id)
            .ValueGeneratedOnAdd()
            .HasValueGenerator<GuidV7ValueGenerator>();
        
        builder.HasOne(m => m.TodoItem)
            .WithMany(m => m.TodoItemShares)
            .HasForeignKey(m => new {m.TodoItemId, m.UserId})
            .OnDelete(DeleteBehavior.Cascade);
    }
}