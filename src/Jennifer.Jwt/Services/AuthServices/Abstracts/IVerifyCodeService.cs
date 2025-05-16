using Jennifer.Jwt.Services.AuthServices.Contracts;
using Jennifer.Jwt.Services.Bases;

namespace Jennifer.Jwt.Services.AuthServices.Abstracts;

public interface IVerifyCodeService: IServiceBase<VerifyCodeRequest, VerifyCodeResponse>
{
    
}