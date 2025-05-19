using Jennifer.Jwt.Application.Auth.Services.Abstracts;
using Jennifer.Jwt.Application.Auth.Services.Contracts;
using Jennifer.Jwt.Models;
using Jennifer.Jwt.Models.Contracts;
using Jennifer.SharedKernel;
using Microsoft.Extensions.Logging;

namespace Jennifer.Jwt.Application.Auth.Commands.SignUp;

public sealed record SignUpDomainEvent(User User): IDomainEvent;

public class SignUpDomainEventHandler(ILogger<SignUpDomainEventHandler> logger,
    IVerifyCodeSendEmailService sendVerifyCodeService): IDomainEventHandler<SignUpDomainEvent>
{
    public async Task Handle(SignUpDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        logger.LogDebug("Domain Event:{UserId}", domainEvent.User.Id);
        
        await sendVerifyCodeService.HandleAsync(new VerifyCodeSendEmailRequest(domainEvent.User.Email, domainEvent.User.UserName, ENUM_EMAIL_VERIFICATION_TYPE.SIGN_UP_BEFORE), cancellationToken);
    }
}