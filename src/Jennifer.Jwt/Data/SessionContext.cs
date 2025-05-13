using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Jennifer.Jwt.Data;

public interface ISessionContext
{
    string Email { get; set; }
    string UserId { get; set; }
    bool IsAuthenticated { get; }
    JenniferDbContext DbContext { get; }
}

public class SessionContext : ISessionContext
{
    public string Email { get; set; }
    public string UserId { get; set; }
    public bool IsAuthenticated => !string.IsNullOrEmpty(UserId);
    public JenniferDbContext DbContext { get; set; }

    public SessionContext(JenniferDbContext dbContext = null) =>
        DbContext = dbContext ?? new JenniferDbContext(new DbContextOptions<JenniferDbContext>());
}

public class SessionContextMiddleware
{
    private readonly RequestDelegate _next;

    public SessionContextMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var session = context.Request.HttpContext.RequestServices.GetRequiredService<ISessionContext>();
            var sid = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(sid, out var guid))
            {
                session.UserId = sid;

                var user = await session.DbContext.Users
                    .AsNoTracking()
                    .Where(u => u.Id == guid)
                    .Select(u => new { u.Email })
                    .FirstOrDefaultAsync();

                if (user != null)
                    session.Email = user.Email;
            }
        }
        await _next(context);
    }
}