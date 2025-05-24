using Jennifer.Account.Application.Auth.Contracts;
using Jennifer.Infrastructure.Abstractions.ServiceCore;
using Jennifer.SharedKernel;
using Microsoft.AspNetCore.Http;

namespace Jennifer.Account.Application.Auth.Services.Abstracts;

internal interface IVerifyCodeSendEmailService : IServiceBase<VerifyCodeSendEmailRequest, Result>
{
    
}