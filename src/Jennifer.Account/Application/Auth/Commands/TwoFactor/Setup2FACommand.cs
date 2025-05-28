using Jennifer.SharedKernel;
using Mediator;

namespace Jennifer.Account.Application.Auth.Commands.TwoFactor;

internal sealed record Setup2FACommand(Guid UserId) : ICommand<Result<Setup2FAResult>>;

internal sealed record Setup2FAResult(string SecretKey, string QrCodeBase64);