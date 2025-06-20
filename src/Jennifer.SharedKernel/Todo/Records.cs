namespace Jennifer.SharedKernel.Todo;

/// <summary>
/// Request model for getting todo items with pagination and description filter
/// </summary>
public sealed record GetsTodoRequest(string Description, int PageNo, int PageSize);

/// <summary>
/// Request model for getting a single todo item by ID
/// </summary>
public sealed record GetTodoRequest(Guid Id);