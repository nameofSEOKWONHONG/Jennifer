using Jennifer.Jwt.Domains;
using Jennifer.SharedKernel;

namespace Jennifer.Jwt.Services.UserServices.Abstracts;

public interface IGetUserService : IServiceBase<Guid, ApiResponse<UserDto>>
{
    
}