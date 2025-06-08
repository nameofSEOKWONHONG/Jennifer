using Jennifer.SharedKernel;
using Mediator;

namespace Jennifer.Account.Application.Users.Commands;

public sealed record AddOrUpdateUserRoleCommand(Guid UserId, Guid RoleId): ICommand<Result>;