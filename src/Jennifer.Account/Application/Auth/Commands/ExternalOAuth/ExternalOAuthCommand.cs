using Jennifer.Account.Application.Auth.Contracts;
using Jennifer.SharedKernel;
using Mediator;

namespace Jennifer.Account.Application.Auth.Commands.ExternalOAuth;

internal sealed record ExternalOAuthCommand(string Provider, string AccessToken):ICommand<Result<TokenResponse>>;