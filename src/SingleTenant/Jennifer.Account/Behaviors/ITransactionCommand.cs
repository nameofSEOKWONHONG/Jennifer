using Mediator;

namespace Jennifer.Account.Behaviors;

public interface ITransactionCommand : ICommand
{
}

public interface ITransactionCommand<TResponse> : ICommand<TResponse>
{
}