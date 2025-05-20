using Jennifer.Jwt.Models;

namespace Jennifer.Jwt.Session.Abstracts;

public interface IUserContext
{
    string UserId { get; }
    string UserName { get; }
    
    Guid UserGuid { get; }

    void SetContext(string userName);

    Task<IEnumerable<UserRole>> GetUserRolesAsync();
}