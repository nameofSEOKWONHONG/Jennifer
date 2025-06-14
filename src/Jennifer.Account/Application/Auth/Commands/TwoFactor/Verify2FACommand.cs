using Jennifer.SharedKernel;
using Jennifer.SharedKernel.Account.Auth;
using Mediator;

namespace Jennifer.Account.Application.Auth.Commands.TwoFactor;

public sealed record Verify2FaCommand(Guid UserId, string Code) : ICommand<Result<TokenResponse>>;