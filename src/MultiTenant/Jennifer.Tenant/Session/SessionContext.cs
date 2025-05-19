using System.Security.Claims;
using eXtensionSharp;
using Jennifer.SharedKernel;
using Jennifer.Tenant.Data;
using Jennifer.Tenant.Session.Abstracts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Tenant.Session;

public class TenantSessionContext : ITenantSessionContext, ITenantSessionContextInitializer
{
    public string TenantId { get; }
    public Guid TenantGuid => Guid.Parse(TenantId);
    public IUserContext UserContext { get; }
    public Guid UserGuid => Guid.Parse(UserContext.UserId);
    public bool IsAuthenticated => !string.IsNullOrEmpty(UserContext?.UserId);
    public IApplicationDbContext ApplicationDbContext { get; set; }

    public TenantSessionContext(IHttpContextAccessor accessor, IUserContext userContext)
    {
        var tenantId = accessor.HttpContext.User.FindFirstValue("tenantId");
        this.TenantId = tenantId;
        
        UserContext = userContext;
    }

    public async Task Initialize(IApplicationDbContext applicationDbContext)
    {
        this.ApplicationDbContext = applicationDbContext;
        
        var user = await applicationDbContext.xAs<JenniferTenantDbContext>().Users
            .AsNoTracking()
            .Where(m => m.TenantId == this.TenantGuid)
            .Where(u => u.Id == UserContext.UserGuid)
            .Select(u => new { u.Email, u.UserName })
            .FirstAsync();
        
        UserContext.SetContext(user.UserName);
    }
}