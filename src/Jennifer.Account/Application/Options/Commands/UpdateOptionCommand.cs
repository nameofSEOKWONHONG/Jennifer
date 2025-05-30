using eXtensionSharp;
using Jennifer.Domain.Accounts.Contracts;
using Jennifer.Infrastructure.Database;
using Jennifer.Infrastructure.Session.Abstracts;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Account.Application.Options.Commands;

public sealed record UpdateOptionRequest(int Id, ENUM_OPTION_TYPE Type, string Value);
public sealed record UpdateOptionCommand(int Id, ENUM_OPTION_TYPE Type, string Value): ICommand<Result>;
