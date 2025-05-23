using Jennifer.SharedKernel;
using Mediator;

namespace Jennifer.Account.Application.Users.Commands;

internal sealed record RemoveUserCommand(Guid UserId) : ICommand<Result>;