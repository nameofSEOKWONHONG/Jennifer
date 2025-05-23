using Jennifer.Account.Application.Auth.Contracts;
using Jennifer.SharedKernel;

namespace Jennifer.Account.Application.Auth.Services.Abstracts;

internal interface IVerifyCodeConfirmService: IServiceBase<VerifyCodeRequest, VerifyCodeResponse>
{
    
}