using Jennifer.Jwt.Domains;
using Jennifer.SharedKernel.Base;
using Jennifer.SharedKernel.Domains;

namespace Jennifer.Jwt.Services.UserServices.Abstracts;

public interface IGetUserService : IServiceBase<Guid, ApiResponse<UserDto>>
{
    
}