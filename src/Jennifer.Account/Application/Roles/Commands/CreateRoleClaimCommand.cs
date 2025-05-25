using Jennifer.SharedKernel;
using Mediator;

namespace Jennifer.Account.Application.Roles.Commands;

public sealed record CreateRoleClaimRequest(string ClaimType, string ClaimValue);
internal sealed record CreateRoleClaimCommand(Guid RoleId, CreateRoleClaimRequest[] requests):ICommand<Result>;