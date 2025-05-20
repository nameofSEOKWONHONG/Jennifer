using Jennifer.Infrastructure.Abstractions.Messaging;

namespace Jennifer.Jwt.Application.Users.Commands;

public sealed record RemoveUserCommand(Guid UserId) : ICommand;