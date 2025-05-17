using System.Security.Claims;
using eXtensionSharp;
using Jennifer.Jwt.Data;
using Jennifer.Jwt.Infrastructure.Session.Abstracts;
using Jennifer.SharedKernel;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Jennifer.Jwt.Infrastructure.Session;

public class JenniferSessionContextMiddleware
{
    private readonly RequestDelegate _next;

    public JenniferSessionContextMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, 
        JenniferDbContext applicationDbContext)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var session = context.Request.HttpContext.RequestServices.GetRequiredService<ISessionContext>();
            var sid = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(sid, out var guid))
            {
                var user = await applicationDbContext.Users
                    .AsNoTracking()
                    .Where(u => u.Id == guid)
                    .Select(u => new { u.Id, u.Email })
                    .FirstAsync();
                
                var sessionInitializer = session.xAs<ISessionContextInitializer>();
                sessionInitializer.Initialize(user.Id.ToString(), user.Email, applicationDbContext);
            }
        }
        await _next(context);
    }
}