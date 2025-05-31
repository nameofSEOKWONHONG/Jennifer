using Mediator;

namespace Jennifer.Domain.Todos;

public sealed record TodoItemShareEvent(Guid Id, Guid ShareUserId):INotification;