using eXtensionSharp;
using Jennifer.Jwt.Data;
using Jennifer.Jwt.Infrastructure.Session.Abstracts;
using Jennifer.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Jwt.Infrastructure.Session;

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