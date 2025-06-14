using Jennifer.SharedKernel;
using Jennifer.SharedKernel.Account.Role;
using Mediator;

namespace Jennifer.Account.Application.Roles.Queries;

public sealed record GetsRoleRequest(string RoleName, int PageNo =1, int PageSize = 10);
public sealed record GetsRoleQuery(string RoleName, int PageNo = 1, int PageSize = 10) : IQuery<PaginatedResult<RoleDto>>;