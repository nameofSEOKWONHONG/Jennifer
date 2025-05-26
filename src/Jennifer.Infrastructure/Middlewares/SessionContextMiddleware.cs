using System.Security.Claims;
using eXtensionSharp;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;

namespace Jennifer.Infrastructure.Middlewares;

public class SessionContextMiddleware
{
    private readonly RequestDelegate _next;

    public SessionContextMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context,
        IDistributedCache cache)
    {
        await _next(context);
        
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userid = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var exists = await cache.GetAsync(userid);
            if (exists.xIsNotEmpty())
            {
                await cache.RefreshAsync(userid);    
            }
        }        
    }
}