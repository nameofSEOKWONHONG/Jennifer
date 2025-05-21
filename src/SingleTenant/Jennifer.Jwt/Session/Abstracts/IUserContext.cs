using Jennifer.Jwt.Models;

namespace Jennifer.Jwt.Session.Abstracts;

public interface IUserContext
{
    string UserId { get; }
    
    Guid UserGuid { get; }

    Task<IEnumerable<UserRole>> GetUserRolesAsync();
    Task<User> GetUserAsync();
}