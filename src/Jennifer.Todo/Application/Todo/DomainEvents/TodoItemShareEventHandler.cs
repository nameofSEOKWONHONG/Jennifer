using Confluent.Kafka;
using Jennifer.Domain.Todos;
using Mediator;
using Microsoft.Extensions.Logging;

namespace Jennifer.Todo.Application.Todo.DomainEvents;

public sealed class TodoItemShareEventHandler(
    ILogger<TodoItemShareEventHandler> logger,
    IProducer<string, string> producer): INotificationHandler<TodoItemShareEvent>
{
    public async ValueTask Handle(TodoItemShareEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("TodoItemShareEvent: documentId:{documentId}, receiverId:{receiverId}", notification.Id, notification.ShareUserId);

        var msg = new Message<string, string>();
        msg.Key = notification.Id.ToString();
        msg.Value = notification.ShareUserId.ToString();
        
        await producer.ProduceAsync("todo-item-share", msg, cancellationToken);
    }
}