using Jennifer.SharedKernel;
using Mediator;

namespace Jennifer.Jwt.Application.Users.Commands;

public sealed record RemoveUserCommand(Guid UserId) : ICommand<Result>;