using Jennifer.SharedKernel;
using Mediator;

namespace Jennifer.Account.Application.Auth.Commands.TwoFactor;

public sealed record Setup2FaCommand(Guid UserId) : ICommand<Result<Setup2FaResult>>;

public sealed record Setup2FaResult(string SecretKey, string QrCodeBase64);