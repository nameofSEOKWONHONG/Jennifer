using Jennifer.Domain.Accounts.Contracts;
using Jennifer.SharedKernel;
using Mediator;

namespace Jennifer.Account.Application.Options.Commands;

public sealed record CreateOptionRequest(ENUM_OPTION_TYPE Type, string Value);
public sealed record CreateOptionCommand(ENUM_OPTION_TYPE type, string Value): ICommand<Result<int>>;
