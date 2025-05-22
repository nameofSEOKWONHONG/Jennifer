using eXtensionSharp;
using Jennifer.Account.Data;
using Jennifer.Account.Session.Abstracts;
using Microsoft.AspNetCore.Http;

namespace Jennifer.Account.Session;

public class JenniferSessionContextMiddleware
{
    private readonly RequestDelegate _next;

    public JenniferSessionContextMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context,
        ISessionContext sessionContext,
        JenniferDbContext applicationDbContext)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var sessionInitializer = sessionContext.xAs<ISessionContextInitializer>();
            await sessionInitializer.Initialize(applicationDbContext);
        }
        await _next(context);
    }
}