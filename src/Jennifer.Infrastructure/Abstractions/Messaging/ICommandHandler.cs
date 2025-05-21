﻿// using Jennifer.SharedKernel;
//
// namespace Jennifer.Infrastructure.Abstractions.Messaging;
//
// public interface ICommandHandler<in TCommand>
//     where TCommand : ICommand
// {
//     Task<Result> HandleAsync(TCommand command, CancellationToken cancellationToken);
// }
//
// public interface ICommandHandler<in TCommand, TResponse>
//     where TCommand : ICommand<TResponse>
// {
//     Task<Result<TResponse>> HandleAsync(TCommand command, CancellationToken cancellationToken);
// }
