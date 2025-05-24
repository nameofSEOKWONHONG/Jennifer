using Mediator;

namespace Jennifer.Infrastructure.Abstractions.Behaviors;

public interface ITransactionCommand : ICommand
{
}

public interface ITransactionCommand<TResponse> : ICommand<TResponse>
{
}