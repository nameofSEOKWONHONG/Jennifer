using Jennifer.Domain.Todos;

namespace Jennifer.Todo.Application.Todo.Contracts;

public sealed record GetsTodoRequest(string Description, int PageNo, int PageSize);

public sealed record GetTodoRequest(Guid Id);

public sealed record TodoItemDto(Guid Id, Guid UserId, string Description, DateTime? DueDate, List<string> Labels, bool IsCompleted, DateTime? CompletedAt, Priority Priority);