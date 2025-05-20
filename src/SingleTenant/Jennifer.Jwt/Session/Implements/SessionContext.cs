using eXtensionSharp;
using Jennifer.Infrastructure.Data;
using Jennifer.Jwt.Data;
using Jennifer.Jwt.Session.Abstracts;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Jwt.Session.Implements;

public class SessionContext : ISessionContext, ISessionContextInitializer
{
    public IUserContext UserContext { get; }
    public Guid UserGuid => Guid.Parse(UserContext.UserId);
    public bool IsAuthenticated => !string.IsNullOrEmpty(UserContext?.UserId);
    public IApplicationDbContext ApplicationDbContext { get; set; }

    public SessionContext(IUserContext userContext)
    {
        UserContext = userContext;
    }

    public async Task Initialize(IApplicationDbContext applicationDbContext)
    {
        this.ApplicationDbContext = applicationDbContext;
        
        var user = await applicationDbContext.xAs<JenniferDbContext>().Users
            .AsNoTracking()
            .Where(u => u.Id == UserContext.UserGuid)
            .Select(u => new { u.Email, u.UserName })
            .FirstAsync();
        
        UserContext.SetContext(user.UserName);
    }
}