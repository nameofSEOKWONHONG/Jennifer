using Jennifer.Infrastructure.Abstractions.Messaging;
using Jennifer.SharedKernel;

namespace Jennifer.Jwt.Application.Auth.Commands.CheckEmail;

public sealed record DeleteUserCommand(Guid UserId) : ICommand;

internal class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand>
{
    public Task<Result> HandleAsync(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}