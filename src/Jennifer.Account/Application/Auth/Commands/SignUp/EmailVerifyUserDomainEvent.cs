using Jennifer.Account.Application.Auth.Contracts;
using Jennifer.Account.Application.Auth.Services.Abstracts;
using Jennifer.Account.Models;
using Jennifer.Account.Models.Contracts;
using Jennifer.Infrastructure.Abstractions.Behaviors;
using Jennifer.Infrastructure.Abstractions.ServiceCore;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.Extensions.Logging;

namespace Jennifer.Account.Application.Auth.Commands.SignUp;

public sealed record EmailVerifyUserDomainEvent(User User): IDomainEvent;

internal sealed class EmailVerifyUserDomainEventHandler(ILogger<EmailVerifyUserDomainEventHandler> logger,
    IServiceExecutionBuilderFactory factory): INotificationHandler<EmailVerifyUserDomainEvent>
{
    public async ValueTask Handle(EmailVerifyUserDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        logger.LogDebug("Domain Event:{UserId}", domainEvent.User.Id);
        
        Result result = null;
        var builder = factory.Create();
        await builder.Register<IVerifyCodeSendEmailService, VerifyCodeSendEmailRequest, Result>()
            .Request(new VerifyCodeSendEmailRequest(domainEvent.User.Email, domainEvent.User.UserName,
                ENUM_EMAIL_VERIFICATION_TYPE.SIGN_UP_BEFORE))
            .Handle(r => result = r)
            .ExecuteAsync(cancellationToken);
        
        if(!result.IsSuccess)
            logger.LogCritical("Email Send Fail.:{message}", result.Message);;
    }
}