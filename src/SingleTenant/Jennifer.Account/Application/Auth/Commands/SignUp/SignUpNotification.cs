using Jennifer.Account.Application.Auth.Contracts;
using Jennifer.Account.Application.Auth.Services.Abstracts;
using Jennifer.Account.Models;
using Jennifer.Account.Models.Contracts;
using Mediator;
using Microsoft.Extensions.Logging;

namespace Jennifer.Account.Application.Auth.Commands.SignUp;

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