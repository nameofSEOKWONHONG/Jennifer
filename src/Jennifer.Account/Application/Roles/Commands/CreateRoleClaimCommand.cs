using Jennifer.SharedKernel;
using Jennifer.SharedKernel.Account.Role;
using Mediator;

namespace Jennifer.Account.Application.Roles.Commands;


public sealed record CreateRoleClaimCommand(Guid RoleId, CreateRoleClaimRequest[] requests):ICommand<Result>;