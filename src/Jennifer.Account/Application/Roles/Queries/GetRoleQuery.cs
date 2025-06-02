using Jennifer.Account.Application.Roles.Contracts;
using Jennifer.SharedKernel;
using Mediator;

namespace Jennifer.Account.Application.Roles.Queries;

public sealed record GetRoleQuery(Guid Id):IQuery<Result<RoleDto>>;
