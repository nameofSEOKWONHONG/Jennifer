using Mediator;
using Microsoft.Extensions.Logging;

namespace Jennifer.Jwt.Behaviors;

public sealed record ErrorMessage(Exception Exception);

public class NotificationBehavior<TMessage, TResponse>: IPipelineBehavior<TMessage, TResponse>
    where TMessage : IMessage // Constrained to IMessage, or constrain to IBaseCommand or any custom interface you've implemented
{
    private readonly ILogger<NotificationBehavior<TMessage, TResponse>> _logger;
    private readonly IMediator _mediator;

    public NotificationBehavior(ILogger<NotificationBehavior<TMessage, TResponse>> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public async ValueTask<TResponse> Handle(TMessage message, MessageHandlerDelegate<TMessage, TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            var response = await next(message, cancellationToken);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling message");
            await _mediator.Publish(new ErrorMessage(ex));
            throw;
        }
    }
}