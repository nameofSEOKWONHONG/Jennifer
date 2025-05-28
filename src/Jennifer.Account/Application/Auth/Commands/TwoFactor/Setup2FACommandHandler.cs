using eXtensionSharp;
using Jennifer.Account.Data;
using Jennifer.Account.Models;
using Jennifer.Account.Models.Contracts;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QRCoder;

namespace Jennifer.Account.Application.Auth.Commands.TwoFactor;

/// <summary>
/// Handles the execution of the Setup2FACommand, which sets up two-factor authentication (2FA)
/// for a user. This command generates a secret key and a corresponding QR code that can be scanned
/// by an authenticator application to enable 2FA for the user.
/// </summary>
internal sealed class Setup2FACommandHandler(UserManager<User> userManager,
    JenniferDbContext dbContext) : ICommandHandler<Setup2FACommand, Result<Setup2FAResult>>
{
    public async ValueTask<Result<Setup2FAResult>> Handle(Setup2FACommand command, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(command.UserId.ToString());
        if (user.xIsEmpty()) 
            return await Result<Setup2FAResult>.FailureAsync("User not found");
        
        if (user.TwoFactorEnabled) 
            return await Result<Setup2FAResult>.FailureAsync("2FA already enabled");
        
        var secretKey = await userManager.GetAuthenticatorKeyAsync(user);
        if (string.IsNullOrEmpty(secretKey))
        {
            await userManager.ResetAuthenticatorKeyAsync(user);
            secretKey = await userManager.GetAuthenticatorKeyAsync(user);
        }
        
        var otpUri = $"otpauth://totp/Jennifer:{user.Email}?secret={secretKey}&issuer=Jennifer";
        var templateOtpUri = await dbContext.Options.AsNoTracking().FirstOrDefaultAsync(m => m.Type == ENUM_ACCOUNT_OPTION.OTP_URI, cancellationToken: cancellationToken);
        if (templateOtpUri.xIsNotEmpty()) otpUri = templateOtpUri.Value;

        using var qrGen = new QRCodeGenerator();
        using var qrCodeData = qrGen.CreateQrCode(otpUri, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);
        var qrBytes = qrCode.GetGraphic(20);
        var qrBase64 = Convert.ToBase64String(qrBytes);

        return await Result<Setup2FAResult>.SuccessAsync(new Setup2FAResult(secretKey, qrBase64));
    }
}