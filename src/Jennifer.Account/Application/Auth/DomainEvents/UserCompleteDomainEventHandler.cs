﻿using Jennifer.Account.Application.Auth.Services.Abstracts;
using Jennifer.Domain.Accounts;
using Jennifer.Domain.Accounts.Contracts;
using Jennifer.Infrastructure.Abstractions.ServiceCore;
using Jennifer.SharedKernel;
using Jennifer.SharedKernel.Account.Auth;
using Mediator;
using Microsoft.Extensions.Logging;

namespace Jennifer.Account.Application.Auth.DomainEvents;

public sealed class UserCompleteDomainEventHandler(ILogger<UserCompleteDomainEventHandler> logger,
    IServiceExecutionBuilderFactory factory): INotificationHandler<UserCompleteDomainEvent>
{
    public async ValueTask Handle(UserCompleteDomainEvent notification, CancellationToken cancellationToken)
    {
        logger.LogDebug("Domain Event:{UserId}", notification.User.Id);
        
        Result result = null;
        var builder = factory.Create();
        await builder.Register<IEmailConfirmSendService, EmailConfirmSendRequest, Result>()
            .Request(new EmailConfirmSendRequest(notification.User.Email, notification.User.UserName,
                ENUM_EMAIL_VERIFY_TYPE.SIGN_UP_BEFORE.Name))
            .Handle(r => result = r)
            .ExecuteAsync(cancellationToken);
        
        if(!result.IsSuccess)
            logger.LogCritical("Email send failed:{Message}", result.Message);
    }
}