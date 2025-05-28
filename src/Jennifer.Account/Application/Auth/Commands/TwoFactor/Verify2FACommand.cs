using Jennifer.Account.Application.Auth.Contracts;
using Jennifer.SharedKernel;
using Mediator;

namespace Jennifer.Account.Application.Auth.Commands.TwoFactor;

internal sealed record Verify2FACommand(Guid UserId, string Code) : ICommand<Result<TokenResponse>>;