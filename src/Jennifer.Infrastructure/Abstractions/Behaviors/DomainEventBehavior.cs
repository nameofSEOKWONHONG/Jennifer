using Mediator;
using Microsoft.Extensions.Logging;

namespace Jennifer.Infrastructure.Abstractions.Behaviors;

public sealed record ErrorMessage(Exception Exception);

public class DomainEventBehavior<TMessage, TResponse>: IPipelineBehavior<TMessage, TResponse>
    where TMessage : IMessage // Constrained to IMessage, or constrain to IBaseCommand or any custom interface you've implemented
{
    private readonly ILogger<DomainEventBehavior<TMessage, TResponse>> _logger;
    private readonly IDomainEventPublisher _domainEventPublisher;

    public DomainEventBehavior(ILogger<DomainEventBehavior<TMessage, TResponse>> logger,
        IDomainEventPublisher domainEventPublisher)
    {
        _logger = logger;
        _domainEventPublisher = domainEventPublisher;
    }

    public async ValueTask<TResponse> Handle(TMessage message, MessageHandlerDelegate<TMessage, TResponse> next, CancellationToken cancellationToken)
    {
        var result = await next(message, cancellationToken);
        
        if (!_domainEventPublisher.IsEmpty())
        {
            await _domainEventPublisher.PublishEnqueuedAsync(cancellationToken);
        }

        return result;
    }
}