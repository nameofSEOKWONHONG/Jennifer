using Jennifer.SharedKernel;
using Mediator;

namespace Jennifer.Account.Application.Auth.Commands.Password;

internal sealed record PasswordForgotVerifyCommand(string Email, string Code) : ICommand<Result>;


