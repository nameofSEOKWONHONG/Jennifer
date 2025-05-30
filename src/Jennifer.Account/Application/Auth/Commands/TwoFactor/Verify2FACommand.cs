using Jennifer.Account.Application.Auth.Contracts;
using Jennifer.SharedKernel;
using Mediator;

namespace Jennifer.Account.Application.Auth.Commands.TwoFactor;

public sealed record Verify2FaCommand(Guid UserId, string Code) : ICommand<Result<TokenResponse>>;