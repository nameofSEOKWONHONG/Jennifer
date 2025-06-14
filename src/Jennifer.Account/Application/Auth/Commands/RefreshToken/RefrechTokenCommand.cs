using Jennifer.SharedKernel;
using Jennifer.SharedKernel.Account.Auth;
using Mediator;

namespace Jennifer.Account.Application.Auth.Commands.RefreshToken;

public sealed record RefreshTokenRequest(string Token);
public sealed record RefreshTokenCommand(string Token):ICommand<Result<TokenResponse>>;



