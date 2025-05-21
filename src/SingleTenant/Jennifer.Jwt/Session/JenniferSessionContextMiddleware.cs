using eXtensionSharp;
using Jennifer.Jwt.Data;
using Jennifer.Jwt.Session.Abstracts;
using Microsoft.AspNetCore.Http;

namespace Jennifer.Jwt.Session;

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
            //...
        }
        await _next(context);
    }
}