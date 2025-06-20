using FluentValidation;

namespace Jennifer.SharedKernel.Todo;

/// <summary>
/// DTO model representing a todo item with all its properties
/// </summary>
public sealed class TodoItemDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime? DueDate{ get; set; }
    public List<string> Labels{ get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt{ get; set; }
    public int Priority { get; set; }
    public List<Guid> SharedUsers { get; set; } = new();
}

public sealed class TodoItemDtoValidator : AbstractValidator<TodoItemDto>
{
    public TodoItemDtoValidator()
    {
        RuleFor(m => m.Title)
            .NotEmpty()
            .MaximumLength(200);
        
        RuleFor(m => m.Description)
            .NotEmpty()
            .MaximumLength(8000);
    }
}