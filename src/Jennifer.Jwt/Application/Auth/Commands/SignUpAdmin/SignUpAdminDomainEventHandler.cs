using Jennifer.SharedKernel;
using Microsoft.Extensions.Logging;

namespace Jennifer.Jwt.Application.Auth.Commands.SignUpAdmin;

internal sealed class SignUpAdminDomainEventHandler(ILogger<SignUpAdminDomainEventHandler> logger) : IDomainEventHandler<SignUpAdminDomainEvent>
{
    public Task Handle(SignUpAdminDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        logger.LogDebug("Domain Event:{UserId}", domainEvent.UserId);
        
        //send welcome email. or send an email check 
        
        return Task.CompletedTask;
    }
}