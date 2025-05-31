using Jennifer.Domain.Todos;

namespace Jennifer.Todo.Application.Todo.Contracts;

/// <summary>
/// Request model for getting todo items with pagination and description filter
/// </summary>
public sealed record GetsTodoRequest(string Description, int PageNo, int PageSize);

/// <summary>
/// Request model for getting a single todo item by ID
/// </summary>
public sealed record GetTodoRequest(Guid Id);

/// <summary>
/// DTO model representing a todo item with all its properties
/// </summary>
public sealed class TodoItemDto
{
    public Guid Id { get; set; }
    public     Guid UserId{ get; set; }
    public string Description{ get; set; }
    public     DateTime? DueDate{ get; set; }
    public List<string> Labels{ get; set; }
    public bool IsCompleted{ get; set; }
    public     DateTime? CompletedAt{ get; set; }
    public Priority Priority { get; set; }
    public List<Guid> SharedUsers { get; set; } = new();
}