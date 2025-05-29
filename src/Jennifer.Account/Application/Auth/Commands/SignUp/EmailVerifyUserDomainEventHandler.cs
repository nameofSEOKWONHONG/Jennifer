using Jennifer.Account.Application.Auth.Contracts;
using Jennifer.Account.Application.Auth.Services.Abstracts;
using Jennifer.Domain.Account;
using Jennifer.Domain.Account.Contracts;
using Jennifer.Infrastructure.Abstractions.ServiceCore;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.Extensions.Logging;

namespace Jennifer.Account.Application.Auth.Commands.SignUp;

internal sealed class UserCompleteDomainEventHandler(ILogger<UserCompleteDomainEventHandler> logger,
    IServiceExecutionBuilderFactory factory): IDomainEventHandler<UserCompleteDomainEvent>
{
    public async Task Handle(UserCompleteDomainEvent domainEvent, CancellationToken cancellationToken)
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