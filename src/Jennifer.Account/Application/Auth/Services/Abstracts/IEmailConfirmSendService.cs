using Jennifer.Infrastructure.Abstractions.ServiceCore;
using Jennifer.SharedKernel;
using Jennifer.SharedKernel.Account.Auth;

namespace Jennifer.Account.Application.Auth.Services.Abstracts;

public interface IEmailConfirmSendService : IServiceBase<EmailConfirmSendRequest, Result>
{
    
}