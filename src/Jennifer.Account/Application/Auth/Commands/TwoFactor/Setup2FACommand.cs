using Jennifer.SharedKernel;
using Mediator;

namespace Jennifer.Account.Application.Auth.Commands.TwoFactor;

internal sealed record Setup2FaCommand(Guid UserId) : ICommand<Result<Setup2FaResult>>;

internal sealed record Setup2FaResult(string SecretKey, string QrCodeBase64);