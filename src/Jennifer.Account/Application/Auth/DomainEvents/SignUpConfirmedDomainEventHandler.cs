using Jennifer.Domain.Accounts;
using Jennifer.Todo.Grpc;
using Mediator;
using Microsoft.Extensions.Logging;

namespace Jennifer.Account.Application.Auth.DomainEvents;

public class SignUpConfirmedDomainEventHandler(ILogger<UserCompleteDomainEventHandler> logger,
    TodoService.TodoServiceClient client):
    INotificationHandler<SignUpConfirmedDomainEvent>
{
    public async ValueTask Handle(SignUpConfirmedDomainEvent notification, CancellationToken cancellationToken)
    {
        var res = await client.SyncUserInfoAsync(new UserData()
        {
            UserId = notification.User.Id.ToString(),
            Email = notification.User.Email,
            UserName = notification.User.UserName,
            Type = notification.User.Type.Value,
        });

        if (!res.Success)
        {
            logger.LogWarning("SignUpConfirmedDomainEventHandler failed: {UserId}", notification.User.Id);
            //TODO: outbox 구분해야 함.
        }
    }
}