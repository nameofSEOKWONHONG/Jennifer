using Jennifer.Jwt.Application.Auth.Services.Contracts;
using Jennifer.SharedKernel;

namespace Jennifer.Jwt.Services.UserServices.Abstracts;

public interface IGetUserService : IServiceBase<Guid, ApiResponse<UserDto>>
{
    
}