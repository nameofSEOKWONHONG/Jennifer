using System.Security.Claims;
using eXtensionSharp;
using Jennifer.Account.Data;
using Jennifer.Account.Session.Abstracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;

namespace Jennifer.Account.Session;

internal class JenniferSessionContextMiddleware
{
    private readonly RequestDelegate _next;

    public JenniferSessionContextMiddleware(RequestDelegate next) => _next = next;

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