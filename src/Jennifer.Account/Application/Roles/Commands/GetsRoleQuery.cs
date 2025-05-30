using Jennifer.Account.Application.Roles.Contracts;
using Jennifer.SharedKernel;
using Mediator;

namespace Jennifer.Account.Application.Roles.Commands;

public sealed record GetsRoleRequest(string RoleName, int PageNo =1, int PageSize = 10);
public sealed record GetsRoleQuery(string RoleName, int PageNo = 1, int PageSize = 10) : IQuery<PaginatedResult<RoleDto>>;