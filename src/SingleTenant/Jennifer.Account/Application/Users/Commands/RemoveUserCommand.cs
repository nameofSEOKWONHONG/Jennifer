using Jennifer.SharedKernel;
using Mediator;

namespace Jennifer.Account.Application.Users.Commands;

public sealed record RemoveUserCommand(Guid UserId) : ICommand<Result>;