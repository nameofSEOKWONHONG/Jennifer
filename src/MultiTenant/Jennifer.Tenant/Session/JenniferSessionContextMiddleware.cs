using eXtensionSharp;
using Jennifer.Tenant.Data;
using Jennifer.Tenant.Session.Abstracts;
using Microsoft.AspNetCore.Http;

namespace Jennifer.Tenant.Session;

public class JenniferSessionContextMiddleware
{
    private readonly RequestDelegate _next;

    public JenniferSessionContextMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context,
        ITenantSessionContext sessionContext,
        JenniferTenantDbContext applicationDbContext)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var sessionInitializer = sessionContext.xAs<ITenantSessionContextInitializer>();
            await sessionInitializer.Initialize(applicationDbContext);
        }
        await _next(context);
    }
}