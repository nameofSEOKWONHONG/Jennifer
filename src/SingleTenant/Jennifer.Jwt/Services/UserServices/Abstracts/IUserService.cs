using Jennifer.Jwt.Application.Auth.Services.Contracts;
using Jennifer.SharedKernel;

namespace Jennifer.Jwt.Services.UserServices.Abstracts;

public interface IUserService
{
    Task<ApiResponse<IList<UserDto>>> GetUsers(string email, int page, int size, CancellationToken ct);
    Task<ApiResponse<UserDto>> GetUser(Guid id, CancellationToken ct);
    Task<ApiResponse<string>> AddUser(UserDto userAddRequest);
    Task<ApiResponse<bool>> ModifyUser(UserDto userDto, CancellationToken ct);
    Task<ApiResponse<bool>> RemoveUser(Guid id, CancellationToken ct);
}