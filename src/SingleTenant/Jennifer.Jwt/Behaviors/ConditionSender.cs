using Mediator;

namespace Jennifer.Jwt.Behaviors;

/// <summary>
/// 만들었지만 원칙에 맞지 않으므로 사용하지 않는다.
/// </summary>
[Obsolete("Don't use this.", true)]
public class SenderScope
{
    private readonly ISender _sender;

    private SenderScope(ISender sender)
    {
        _sender = sender;
    }

    public static SenderScope Create(ISender sender)
    {
        return new SenderScope(sender);
    }

    public ConditionalQueryBuilder<TQuery, TResponse> AddQuery<TQuery, TResponse>(TQuery query)
        where TQuery : IQuery<TResponse>
    {
        return new ConditionalQueryBuilder<TQuery, TResponse>(_sender, query);
    }
    
    public ConditionalCommandBuilder<TCommand, TResponse> AddCommand<TCommand, TResponse>(TCommand command)
        where TCommand : ICommand<TResponse>
    {
        return new ConditionalCommandBuilder<TCommand, TResponse>(_sender, command);
    }
}

[Obsolete("Don't use this.", true)]
public class ConditionalQueryBuilder<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    private readonly ISender _sender;
    private readonly TQuery _query;
    private Func<bool>? _predicate;

    public ConditionalQueryBuilder(ISender sender, TQuery query)
    {
        _sender = sender;
        _query = query;
    }

    public ConditionalQueryBuilder<TQuery, TResponse> When(Func<bool> predicate)
    {
        _predicate = predicate;
        return this;
    }

    public ConditionalQueryBuilder<TQuery, TResponse> Always()
    {
        _predicate = () => true;
        return this;
    }

    public async Task<TResponse?> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (_predicate?.Invoke() == true)
        {
            return await _sender.Send(_query, cancellationToken);
        }

        return default;
    }
}

[Obsolete("Don't use this.", true)]
public class ConditionalCommandBuilder<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    private readonly ISender _sender;
    private readonly TCommand _command;
    private Func<bool>? _predicate;

    public ConditionalCommandBuilder(ISender sender, TCommand command)
    {
        _sender = sender;
        _command = command;
    }

    public ConditionalCommandBuilder<TCommand, TResponse> When(Func<bool> predicate)
    {
        _predicate = predicate;
        return this;
    }

    public ConditionalCommandBuilder<TCommand, TResponse> Always()
    {
        _predicate = () => true;
        return this;
    }

    public async Task<TResponse?> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (_predicate?.Invoke() == true)
        {
            return await _sender.Send(_command, cancellationToken);
        }

        return default;
    }
}
