using System.Security.Claims;
using eXtensionSharp;
using Jennifer.Infrastructure.Database;
using Jennifer.Infrastructure.Session;
using Jennifer.Infrastructure.Session.Abstracts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;

namespace Jennifer.Infrastructure.Middlewares;

public class SessionMiddleware
{
    private readonly RequestDelegate _next;

    public SessionMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context,
        ISessionContext session)
    {   
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var emailConfirmed = context.User.FindFirstValue("emailConfirmed").xValue<bool>();
            if(!emailConfirmed) throw new Exception("Email is not confirmed");
            
            //TODO: HERE IS ERROR... first succeed, but second fail.
            var cs = context.User.FindFirstValue("cs");
            var user = await session.User.Current.GetAsync();
            var db = context.RequestServices.GetRequiredService<JenniferReadOnlyDbContext>();
            var selectedUser = await db.Users.AsNoTracking().FirstAsync(m => m.Id == user.Id);
            if(cs != selectedUser.ConcurrencyStamp) throw new Exception("ConcurrencyStamp is not matched");
        }
        
        await _next(context);
    }
}