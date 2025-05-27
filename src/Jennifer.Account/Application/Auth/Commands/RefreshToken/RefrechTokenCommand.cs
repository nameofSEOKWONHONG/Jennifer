using Jennifer.Account.Application.Auth.Contracts;
using Jennifer.SharedKernel;
using Mediator;

namespace Jennifer.Account.Application.Auth.Commands.RefreshToken;

public sealed record RefreshTokenCommand(string Token):ICommand<Result<TokenResponse>>;



