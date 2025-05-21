using System.Collections.Concurrent;
using System.Reflection;
using Jennifer.Infrastructure.Abstractions.Messaging;
using Jennifer.SharedKernel;
using Microsoft.Extensions.DependencyInjection;

namespace Jennifer.Infrastructure.Abstractions.Behaviors;

public interface ISender
{
    // Command without return type
    Task<Result> Send(ICommand command, CancellationToken cancellationToken = default);

    // Command with return type
    Task<Result<TResponse>> Send<TResponse>(ICommand<TResponse> command,
        CancellationToken cancellationToken = default);
    
    Task<Result<TResponse>> Send<TResponse>(IQuery<TResponse> query,
        CancellationToken cancellationToken = default);
}

public sealed class Sender : ISender
{
    private readonly IServiceProvider _serviceProvider;

    public Sender(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    private static readonly ConcurrentDictionary<Type, MethodInfo> _methodCache = new();

    public Task<Result> Send(ICommand command, CancellationToken cancellationToken = default)
    {
        return SendInternal<Result>(command, typeof(ICommandHandler<>), cancellationToken);
    }

    public Task<Result<TResponse>> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
    {
        return SendInternal<Result<TResponse>>(command, typeof(ICommandHandler<,>), typeof(TResponse), cancellationToken);
    }

    public Task<Result<TResponse>> Send<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
    {
        return SendInternal<Result<TResponse>>(query, typeof(IQueryHandler<,>), typeof(TResponse), cancellationToken);
    }

    private async Task<T> SendInternal<T>(object message, Type handlerGenericType, CancellationToken ct)
    {
        var handlerType = handlerGenericType.MakeGenericType(message.GetType());
        var handler = _serviceProvider.GetService(handlerType);
        if (handler == null)
        {
            return FailResult<T>(message.GetType().Name);
        }

        var method = _methodCache.GetOrAdd(handlerType, static t => t.GetMethod("HandleAsync")!);
        var task = (Task<T>)method.Invoke(handler, new object[] { message, ct });
        return await task;
    }

    private async Task<T> SendInternal<T>(object message, Type handlerGenericType, Type responseType, CancellationToken ct)
    {
        var handlerType = handlerGenericType.MakeGenericType(message.GetType(), responseType);
        var handler = _serviceProvider.GetService(handlerType);
        if (handler == null)
        {
            return FailResult<T>(message.GetType().Name);
        }

        var method = _methodCache.GetOrAdd(handlerType, static t => t.GetMethod("HandleAsync")!);
        var task = (Task<T>)method.Invoke(handler, new object[] { message, ct });
        return await task;
    }

    private static T FailResult<T>(string typeName)
    {
        var error = Error.Failure("Handler.NotFound", $"핸들러를 찾을 수 없습니다: {typeName}");
        return typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(Result<>)
            ? (T)Activator.CreateInstance(typeof(Result<>).MakeGenericType(typeof(object)), new object[] { error })!
            : (T)(object)Result.Failure(error);
    }
}