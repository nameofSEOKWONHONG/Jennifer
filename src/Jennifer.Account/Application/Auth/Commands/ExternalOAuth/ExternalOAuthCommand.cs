using Jennifer.Account.Application.Auth.Contracts;
using Jennifer.SharedKernel;
using Mediator;

namespace Jennifer.Account.Application.Auth.Commands.ExternalOAuth;

/// <summary>
/// Command to handle external OAuth authentication
/// </summary>
/// <param name="Provider">The OAuth provider (e.g. Google, Facebook, etc)</param>
/// <param name="AccessToken">OAuth access token from the provider</param> 
public sealed record ExternalOAuthCommand(string Provider, string AccessToken):ICommand<Result<TokenResponse>>;