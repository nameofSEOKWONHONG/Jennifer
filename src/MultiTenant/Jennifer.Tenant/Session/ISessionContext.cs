using System.Security.Claims;
using Jennifer.SharedKernel;
using Jennifer.Tenant.Models;
using Jennifer.Tenant.Session.Abstracts;
using Microsoft.AspNetCore.Http;

namespace Jennifer.Tenant.Session;

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

public interface ITenantSessionContext
{
    public string TenantId { get; }
    public Guid TenantGuid => Guid.Parse(TenantId);
    
    IUserContext UserContext { get; }   
    bool IsAuthenticated { get; }
    IApplicationDbContext ApplicationDbContext { get; }
}