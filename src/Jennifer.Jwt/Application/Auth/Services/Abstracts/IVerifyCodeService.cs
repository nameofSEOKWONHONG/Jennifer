using Jennifer.Jwt.Application.Auth.Services.Contracts;
using Jennifer.SharedKernel;

namespace Jennifer.Jwt.Application.Auth.Services.Abstracts;

public interface IVerifyCodeService: IServiceBase<VerifyCodeRequest, VerifyCodeResponse>
{
    
}