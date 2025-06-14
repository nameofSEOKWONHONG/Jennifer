using Jennifer.SharedKernel;
using Jennifer.SharedKernel.Account.Auth;
using Mediator;

namespace Jennifer.Account.Application.Auth.Commands.SignIn;

public sealed record SignInCommand(string Email, string Password):ICommand<Result<TokenResponse>>;