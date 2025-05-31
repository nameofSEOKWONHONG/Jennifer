using Mediator;

namespace Jennifer.Domain.Todos;

public sealed class TodoItemShareEvent(Guid Id, Guid ShareUserId):INotification;