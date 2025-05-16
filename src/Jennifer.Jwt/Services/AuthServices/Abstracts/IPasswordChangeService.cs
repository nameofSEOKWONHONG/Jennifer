using Jennifer.Jwt.Services.AuthServices.Contracts;
using Jennifer.Jwt.Services.AuthServices.Implements;
using Jennifer.SharedKernel.Base;
using Jennifer.SharedKernel.Domains;

namespace Jennifer.Jwt.Services.AuthServices.Abstracts;

public interface IPasswordChangeService: IServiceBase<PasswordChangeRequest, ApiResponse<bool>>
{
    
}
