using Jennifer.SharedKernel;
using Jennifer.SharedKernel.Account.Auth;
using Mediator;

namespace Jennifer.Account.Application.Auth.Commands.ExternalOAuth;

/// <summary>
/// Command to handle external OAuth authentication
/// </summary>
/// <param name="Provider">The OAuth provider (e.g. Google, Facebook, etc)</param>
/// <param name="AccessToken">OAuth access token from the provider</param> 
public sealed record ExternalOAuthCommand(string Provider, string AccessToken):ICommand<Result<TokenResponse>>;