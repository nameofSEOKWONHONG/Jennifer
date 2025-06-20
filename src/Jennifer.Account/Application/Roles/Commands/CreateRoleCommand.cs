﻿using Jennifer.SharedKernel;
using Mediator;

namespace Jennifer.Account.Application.Roles.Commands;


public sealed record CreateRoleCommand(string RoleName) : ICommand<Result<Guid>>;