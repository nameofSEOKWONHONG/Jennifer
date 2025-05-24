using FluentValidation;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using ICommand = System.Windows.Input.ICommand;

namespace Jennifer.Infrastructure.Abstractions;

public interface ISlimSender
{
    ValueTask<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken ct = default);
    ValueTask<TResponse> Send<TResponse>(IQuery<TResponse> query, CancellationToken ct = default);
    ValueTask Send(ICommand command, CancellationToken ct = default); // 반환 없는 명령
}

public sealed class SlimSender : ISlimSender
{
    private readonly IServiceProvider _provider;

    public SlimSender(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async ValueTask<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken ct = default)
    {
        await ValidateAsync(command, ct);

        var commandType = command.GetType();
        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(commandType, typeof(TResponse));
        var handler = _provider.GetService(handlerType)
                     ?? throw new InvalidOperationException($"No handler registered for {handlerType.FullName}");

        return await InvokeHandler<TResponse>(handler, command, ct);
    }

    public async ValueTask<TResponse> Send<TResponse>(IQuery<TResponse> query, CancellationToken ct = default)
    {
        await ValidateAsync(query, ct);

        var queryType = query.GetType();
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(queryType, typeof(TResponse));
        var handler = _provider.GetService(handlerType)
                     ?? throw new InvalidOperationException($"No handler registered for {handlerType.FullName}");

        return await InvokeHandler<TResponse>(handler, query, ct);
    }

    public async ValueTask Send(ICommand command, CancellationToken ct = default)
    {
        await ValidateAsync(command, ct);

        var commandType = command.GetType();
        var handlerType = typeof(ICommandHandler<>).MakeGenericType(commandType);
        var handler = _provider.GetService(handlerType)
                     ?? throw new InvalidOperationException($"No handler registered for {handlerType.FullName}");

        await InvokeVoidHandler(handler, command, ct);
    }

    private async ValueTask ValidateAsync<TRequest>(TRequest request, CancellationToken ct)
    {
        var validators = _provider.GetServices<IValidator<TRequest>>();
        foreach (var validator in validators)
        {
            var result = await validator.ValidateAsync(request, ct);
            if (!result.IsValid)
                throw new ValidationException(result.Errors);
        }
    }

    private static async ValueTask<TResponse> InvokeHandler<TResponse>(object handler, object request, CancellationToken ct)
    {
        var method = handler.GetType().GetMethod("Handle")
                     ?? throw new MissingMethodException($"Handler {handler.GetType().Name} does not have a Handle method.");

        var resultObj = method.Invoke(handler, new[] { request, ct });

        // 지원: ValueTask<T>
        if (resultObj is ValueTask<TResponse> valueTaskTyped)
        {
            return await valueTaskTyped.ConfigureAwait(false);
        }

        // 지원: Task<T>
        if (resultObj is Task<TResponse> taskTyped)
        {
            return await taskTyped.ConfigureAwait(false);
        }

        throw new InvalidOperationException($"Handler {handler.GetType().Name} returned unexpected type {resultObj?.GetType().Name}");
    }

    private static async ValueTask InvokeVoidHandler(object handler, object request, CancellationToken ct)
    {
        var method = handler.GetType().GetMethod("Handle")
                     ?? throw new MissingMethodException($"Handler {handler.GetType().Name} does not have a Handle method.");

        var resultObj = method.Invoke(handler, new[] { request, ct });

        // 지원: ValueTask
        if (resultObj is ValueTask valueTask)
        {
            await valueTask.ConfigureAwait(false);
            return;
        }

        // 지원: Task
        if (resultObj is Task task)
        {
            await task.ConfigureAwait(false);
            return;
        }

        throw new InvalidOperationException($"Handler {handler.GetType().Name} returned unexpected type {resultObj?.GetType().Name}");
    }
}
