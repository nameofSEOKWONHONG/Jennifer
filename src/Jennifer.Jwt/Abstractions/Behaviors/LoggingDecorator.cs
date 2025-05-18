﻿using Jennifer.Jwt.Abstractions.Messaging;
using Jennifer.SharedKernel;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Jennifer.Jwt.Abstractions.Behaviors;

internal static class LoggingDecorator
{
    internal sealed class CommandHandler<TCommand, TResponse>: ICommandHandler<TCommand, TResponse>
        where TCommand: ICommand<TResponse>
    {
        private readonly ICommandHandler<TCommand, TResponse> _innerHandler;
        private readonly ILogger<TCommand> _logger;

        public CommandHandler(ICommandHandler<TCommand, TResponse> innerHandler, ILogger<TCommand> logger)
        {
            _innerHandler = innerHandler;
            _logger = logger;
        }

        public async Task<Result<TResponse>> HandleAsync(TCommand command, CancellationToken cancellationToken)
        {
            string commandName = typeof(TCommand).Name;

            _logger.LogInformation("Processing command {Command}", commandName);

            Result<TResponse> result = await _innerHandler.HandleAsync(command, cancellationToken);

            if (result.IsSuccess)
            {
                _logger.LogInformation("Completed command {Command}", commandName);
            }
            else
            {
                using (LogContext.PushProperty("Error", result.Error, true))
                {
                    _logger.LogError("Completed command {Command} with error", commandName);
                }
            }

            return result;
        }
    }
    
    internal sealed class CommandBaseHandler<TCommand>: ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly ICommandHandler<TCommand> _innerHandler;
        private readonly ILogger<CommandBaseHandler<TCommand>> _logger;

        public CommandBaseHandler(
            ICommandHandler<TCommand> innerHandler,
            ILogger<CommandBaseHandler<TCommand>> logger)
        {
            _innerHandler = innerHandler;
            _logger = logger;
        }
        
        public async Task<Result> HandleAsync(TCommand command, CancellationToken cancellationToken)
        {
            string commandName = typeof(TCommand).Name;

            _logger.LogInformation("Processing command {Command}", commandName);

            Result result = await _innerHandler.HandleAsync(command, cancellationToken);

            if (result.IsSuccess)
            {
                _logger.LogInformation("Completed command {Command}", commandName);
            }
            else
            {
                using (LogContext.PushProperty("Error", result.Error, true))
                {
                    _logger.LogError("Completed command {Command} with error", commandName);
                }
            }

            return result;
        }
    }

    internal sealed class QueryHandler<TQuery, TResponse>: IQueryHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
        private readonly IQueryHandler<TQuery, TResponse> _innerHandler;
        private readonly ILogger<QueryHandler<TQuery, TResponse>> _logger;

        public QueryHandler(
            IQueryHandler<TQuery, TResponse> innerHandler,
            ILogger<QueryHandler<TQuery, TResponse>> logger)
        {
            _innerHandler = innerHandler;
            _logger = logger;
        }
        
        public async Task<Result<TResponse>> HandleAsync(TQuery query, CancellationToken cancellationToken)
        {
            string queryName = typeof(TQuery).Name;

            _logger.LogInformation("Processing query {Query}", queryName);

            Result<TResponse> result = await _innerHandler.HandleAsync(query, cancellationToken);

            if (result.IsSuccess)
            {
                _logger.LogInformation("Completed query {Query}", queryName);
            }
            else
            {
                using (LogContext.PushProperty("Error", result.Error, true))
                {
                    _logger.LogError("Completed query {Query} with error", queryName);
                }
            }

            return result;
        }
    }    
}