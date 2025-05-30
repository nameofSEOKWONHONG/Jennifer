using Jennifer.SharedKernel;
using Mediator;

namespace Jennifer.Account.Application.Auth.Commands.Password;

public sealed record PasswordForgotVerifyCommand(string Email, string Code) : ICommand<Result>;


