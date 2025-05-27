using Jennifer.Account.Application.Auth.Contracts;
using Jennifer.SharedKernel;
using Mediator;

namespace Jennifer.Account.Application.Auth.Commands.SignIn;

public sealed record SignInCommand(string Email, string Password):ICommand<Result<TokenResponse>>;