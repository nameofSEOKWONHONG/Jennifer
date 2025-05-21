using System.Security.Claims;
using Jennifer.Jwt.Models;
using Jennifer.Jwt.Session.Abstracts;
using Microsoft.AspNetCore.Http;

namespace Jennifer.Jwt.Session.Implements;

public class UserContext : IUserContext
{
    private readonly IUserFetcher _userFetcher;
    private readonly IUserRoleFetcher _userRoleFetcher;
    public string UserId { get; }
    public Guid UserGuid => Guid.Parse(UserId);
    public UserContext(IHttpContextAccessor httpContextAccessor,
        IUserRoleFetcher userRoleFetcher)
    {
        UserId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        _userRoleFetcher = userRoleFetcher;
    }
    
    public async Task<IEnumerable<UserRole>> GetUserRolesAsync() 
        => await _userRoleFetcher.HandleAsync(UserGuid);

    public async Task<User> GetUserAsync() 
        => await _userFetcher.HandleAsync(UserGuid);
}

