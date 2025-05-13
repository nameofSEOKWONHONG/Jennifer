using Jennifer.Domains;
using Jennifer.Jwt.Domains;
using Jennifer.Jwt.Models;

namespace Jennifer.Jwt.Services;

public interface IUserService
{
    Task<IEnumerable<User>> GetUsers(string email, int page, int size, CancellationToken ct);
    Task<User> GetUser(string id, CancellationToken ct);
    Task<ApiResponse<string>> AddUser(UserDto userDto);
    Task<ApiResponse<bool>> ModifyUser(UserDto userDto, CancellationToken ct);
    Task<ApiResponse<bool>> RemoveUser(Guid id, CancellationToken ct);
}