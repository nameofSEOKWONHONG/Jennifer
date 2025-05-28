using Jennifer.Account.Models;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.AspNetCore.Identity;
using OtpNet;
using QRCoder;

namespace Jennifer.Account.Application.Auth.Commands.TwoFactor;

/// <summary>
/// Handles the execution of the Setup2FACommand, which sets up two-factor authentication (2FA)
/// for a user. This command generates a secret key and a corresponding QR code that can be scanned
/// by an authenticator application to enable 2FA for the user.
/// </summary>
/// TODO: 발급자 사항에 따라 otpUri는 변경되어야 함.
internal sealed class Setup2FACommandHandler(UserManager<User> userManager) : ICommandHandler<Setup2FACommand, Result<Setup2FAResult>>
{
    public async ValueTask<Result<Setup2FAResult>> Handle(Setup2FACommand command, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(command.UserId.ToString());
        if (user == null) return await Result<Setup2FAResult>.FailureAsync("User not found");

        var secret = KeyGeneration.GenerateRandomKey(20);
        var secretBase32 = Base32Encoding.ToString(secret);

        user.TwoFactorSecretKey = secretBase32;
        await userManager.UpdateAsync(user);
        
        var otpUri = $"otpauth://totp/Jennifer:{user.Email}?secret={secretBase32}&issuer=Jennifer";

        using var qrGen = new QRCodeGenerator();
        using var qrCodeData = qrGen.CreateQrCode(otpUri, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);
        var qrBytes = qrCode.GetGraphic(20);
        var qrBase64 = Convert.ToBase64String(qrBytes);

        return await Result<Setup2FAResult>.SuccessAsync(new Setup2FAResult(secretBase32, qrBase64));
    }
}