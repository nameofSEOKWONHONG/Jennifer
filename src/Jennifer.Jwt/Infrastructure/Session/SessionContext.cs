using eXtensionSharp;
using Jennifer.Jwt.Infrastructure.Session.Abstracts;
using Jennifer.Jwt.Models;
using Jennifer.SharedKernel;

namespace Jennifer.Jwt.Infrastructure.Session;

public class SessionContext : ISessionContext, ISessionContextInitializer
{
    private readonly IUserRoleFetcher _userRoleFetcher;
    public string Email { get; set; }
    public string UserId { get; set; }
    public Guid UserGuid => Guid.Parse(UserId);
    public bool IsAuthenticated => !string.IsNullOrEmpty(UserId);
    public IApplicationDbContext ApplicationDbContext { get; set; }

    public SessionContext(IUserRoleFetcher userRoleFetcher)
    {
        _userRoleFetcher = userRoleFetcher;
    }

    public void Initialize(string userId, string email, IApplicationDbContext applicationDbContext)
    {
        this.UserId = userId;
        this.Email = email;
        this.ApplicationDbContext = applicationDbContext;
    }
    
    private IEnumerable<UserRole> _userRoles;
    public async Task<IEnumerable<UserRole>> GetUserRolesAsync()
    {
        if (_userRoles.xIsEmpty())
        {
            _userRoles = await _userRoleFetcher.FetchAsync(this.UserGuid);
        }
        return _userRoles;
    }
}