using Jennifer.Jwt.Domains;
using Jennifer.Jwt.Models;
using Jennifer.SharedKernel.Domains;

namespace Jennifer.Jwt.Services.Abstracts;

public interface IUserService
{
    Task<ApiResponse<IList<UserDto>>> GetUsers(string email, int page, int size, CancellationToken ct);
    Task<ApiResponse<UserDto>> GetUser(Guid id, CancellationToken ct);
    Task<ApiResponse<string>> AddUser(RegisterUserDto userDto);
    Task<ApiResponse<bool>> ModifyUser(UserDto userDto, CancellationToken ct);
    Task<ApiResponse<bool>> RemoveUser(Guid id, CancellationToken ct);
}