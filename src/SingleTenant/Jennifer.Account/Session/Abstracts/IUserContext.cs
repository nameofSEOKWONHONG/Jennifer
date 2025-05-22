using Jennifer.Account.Models;

namespace Jennifer.Account.Session.Abstracts;

public interface IUserContext
{
    string UserId { get; }
    string UserName { get; }
    
    Guid UserGuid { get; }

    void SetContext(string userName);

    Task<IEnumerable<UserRole>> GetUserRolesAsync();
}