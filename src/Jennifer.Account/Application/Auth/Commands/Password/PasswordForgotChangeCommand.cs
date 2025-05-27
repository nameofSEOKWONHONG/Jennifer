using Jennifer.SharedKernel;
using Mediator;

namespace Jennifer.Account.Application.Auth.Commands.Password;

internal sealed record PasswordForgotChangeCommand(string Email, string Code, string Password): ICommand<Result>;

