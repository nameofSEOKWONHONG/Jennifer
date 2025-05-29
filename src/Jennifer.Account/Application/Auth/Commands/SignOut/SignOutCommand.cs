using Jennifer.SharedKernel;
using Mediator;

namespace Jennifer.Account.Application.Auth.Commands.SignOut;

public sealed record SignOutCommand(bool dummy):ICommand<Result>;

