using Jennifer.SharedKernel;
using Jennifer.SharedKernel.Account.Role;
using Mediator;

namespace Jennifer.Account.Application.Roles.Queries;

public sealed record GetRoleQuery(Guid Id):IQuery<Result<RoleDto>>;
