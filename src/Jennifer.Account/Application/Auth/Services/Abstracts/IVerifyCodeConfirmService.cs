using Jennifer.Account.Application.Auth.Contracts;
using Jennifer.Infrastructure.Abstractions.ServiceCore;
using Jennifer.SharedKernel;

namespace Jennifer.Account.Application.Auth.Services.Abstracts;

public interface IVerifyCodeConfirmService: IServiceBase<VerifyCodeRequest, Result>
{
    
}