using System.Security.Claims;
using eXtensionSharp;
using Jennifer.Jwt.Data;
using Jennifer.SharedKernel.Infrastructure.Session;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Jennifer.Jwt;

public class JenniferSessionContextMiddleware
{
    private readonly RequestDelegate _next;

    public JenniferSessionContextMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, JenniferDbContext dbContext)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var session = context.Request.HttpContext.RequestServices.GetRequiredService<ISessionContext>();
            var sid = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(sid, out var guid))
            {
                var user = await dbContext.Users
                    .AsNoTracking()
                    .Where(u => u.Id == guid)
                    .Select(u => new { u.Id, u.Email })
                    .FirstAsync();
                
                session.xAs<ISessionContextInitializer>()
                    .Initialize(user.Id.ToString(), user.Email, dbContext);
            }
        }
        await _next(context);
    }
}