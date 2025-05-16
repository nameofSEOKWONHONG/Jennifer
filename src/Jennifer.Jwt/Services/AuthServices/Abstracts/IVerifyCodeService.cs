using Jennifer.Jwt.Services.AuthServices.Contracts;
using Jennifer.SharedKernel.Base;

namespace Jennifer.Jwt.Services.AuthServices.Abstracts;

public interface IVerifyCodeService: IServiceBase<VerifyCodeRequest, VerifyCodeResponse>
{
    
}