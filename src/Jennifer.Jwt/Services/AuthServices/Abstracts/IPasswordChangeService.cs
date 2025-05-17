using Jennifer.Jwt.Services.AuthServices.Contracts;
using Jennifer.SharedKernel;

namespace Jennifer.Jwt.Services.AuthServices.Abstracts;

public interface IPasswordChangeService: IServiceBase<PasswordChangeRequest, ApiResponse<bool>>
{
    
}
