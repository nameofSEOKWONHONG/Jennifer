using System.Security.Claims;
using Jennifer.Jwt.Infrastructure.Session.Abstracts;
using Jennifer.Jwt.Models;
using Jennifer.SharedKernel;
using Microsoft.AspNetCore.Http;

namespace Jennifer.Jwt.Infrastructure.Session;

public interface IUserContext
{
    string UserId { get; }
    string UserName { get; }
    
    Guid UserGuid { get; }

    void SetContext(string userName);
}

public class UserContext : IUserContext
{
    private readonly IUserRoleFetcher _userRoleFetcher;
    public string UserId { get; }
    public string UserName { get; private set; }
    public Guid UserGuid => Guid.Parse(UserId);
    public UserContext(IHttpContextAccessor httpContextAccessor,
        IUserRoleFetcher userRoleFetcher)
    {
        _userRoleFetcher = userRoleFetcher;
        var context = httpContextAccessor.HttpContext;
        UserId = context?.User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
    
    public void SetContext(string userName) => UserName = userName;
    
    public async Task<IEnumerable<UserRole>> GetUserRolesAsync() 
        => await _userRoleFetcher.FetchAsync(UserGuid);
}

public interface ISessionContext
{
    IUserContext UserContext { get; }   
    bool IsAuthenticated { get; }
    IApplicationDbContext ApplicationDbContext { get; }
}