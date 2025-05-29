using eXtensionSharp;
using eXtensionSharp.AspNet;
using Jennifer.Infrastructure.Abstractions.Behaviors;
using Jennifer.SharedKernel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Jennifer.Infrastructure.Middlewares;

public class IpBlockMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<IpBlockMiddleware> _logger;
    private readonly IIpBlockService _service;

    public IpBlockMiddleware(RequestDelegate next, ILogger<IpBlockMiddleware> logger,
        IIpBlockService service)
    {
        _next = next;
        _logger = logger;
        _service = service;
    }

    public async Task Invoke(HttpContext context)
    {
        var ip = context.xGetRemoteIpAddress();
        if (ip.xIsEmpty())
        {
            var result = await Result.FailureAsync("Unable to determine client ip");
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(result);
            
            return;       
        }
        var @checked = await _service.IsBlockedAsync(ip);
        if (@checked)
        {
            var result = await Result.FailureAsync("IP is blocked");
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(result);
            
            return;
        }
        
        await _next(context);
    }
}