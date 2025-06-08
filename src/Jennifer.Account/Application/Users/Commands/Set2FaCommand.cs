using eXtensionSharp;
using Jennifer.Domain.Accounts;
using Jennifer.Infrastructure.Session;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.AspNetCore.Identity;

namespace Jennifer.Account.Application.Users.Commands;

public sealed record Set2FaCommand(Guid UserId, bool enable) : ICommand<Result>;

