﻿// using Jennifer.SharedKernel;
// using Microsoft.Extensions.Logging;
//
// namespace Jennifer.Account.Application.Auth.Commands.SignUp;
//
// internal sealed class SignUpAdminDomainEventHandler(ILogger<SignUpAdminDomainEventHandler> logger) : IDomainEventHandler<SignUpAdminDomainEvent>
// {
//     public Task Handle(SignUpAdminDomainEvent domainEvent, CancellationToken cancellationToken)
//     {
//         logger.LogDebug("Domain Event:{UserId}", domainEvent.UserId);
//         
//         //send welcome email. or send an email check 
//         
//         return Task.CompletedTask;
//     }
// }