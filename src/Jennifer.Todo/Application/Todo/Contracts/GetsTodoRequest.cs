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
public sealed record TodoItemDto(Guid Id, Guid UserId, string Description, DateTime? DueDate, List<string> Labels, bool IsCompleted, DateTime? CompletedAt, Priority Priority);