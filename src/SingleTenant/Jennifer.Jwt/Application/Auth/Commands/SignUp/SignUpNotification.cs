using Jennifer.Jwt.Application.Auth.Contracts;
using Jennifer.Jwt.Application.Auth.Services.Abstracts;
using Jennifer.Jwt.Models;
using Jennifer.Jwt.Models.Contracts;
using Mediator;
using Microsoft.Extensions.Logging;

namespace Jennifer.Jwt.Application.Auth.Commands.SignUp;

public sealed record SignUpNotification(User User): INotification;

public class SignUpNotificationHandler(ILogger<SignUpNotificationHandler> logger,
    IVerifyCodeSendEmailService sendVerifyCodeService): INotificationHandler<SignUpNotification>
{
    public async ValueTask Handle(SignUpNotification domainEvent, CancellationToken cancellationToken)
    {
        logger.LogDebug("Domain Event:{UserId}", domainEvent.User.Id);
        
        await sendVerifyCodeService.HandleAsync(new VerifyCodeSendEmailRequest(domainEvent.User.Email, domainEvent.User.UserName, ENUM_EMAIL_VERIFICATION_TYPE.SIGN_UP_BEFORE), cancellationToken);
    }
}