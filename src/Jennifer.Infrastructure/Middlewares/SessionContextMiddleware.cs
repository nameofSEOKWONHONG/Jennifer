using System.Security.Claims;
using eXtensionSharp;
using Jennifer.Infrastructure.Session;
using Jennifer.Infrastructure.Session.Abstracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;

namespace Jennifer.Infrastructure.Middlewares;

public class SessionContextMiddleware
{
    private readonly RequestDelegate _next;

    public SessionContextMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context,
        ISessionContext session)
    {   
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var emailConfirmed = context.User.FindFirstValue("emailConfirmed").xValue<bool>();
            if(!emailConfirmed) throw new Exception("Email is not confirmed");
            
            // var cs = context.User.FindFirstValue("cs");
            // var user = await session.User.GetAsync();
            // if(cs != user.ConcurrencyStamp) throw new Exception("ConcurrencyStamp is not matched");
        }
        
        await _next(context);
    }
}