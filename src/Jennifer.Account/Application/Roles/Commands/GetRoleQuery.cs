using Jennifer.Account.Application.Roles.Contracts;
using Jennifer.Infrastructure.Database;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Account.Application.Roles.Commands;

public sealed record GetRoleQuery(Guid Id):IQuery<Result<RoleDto>>;
