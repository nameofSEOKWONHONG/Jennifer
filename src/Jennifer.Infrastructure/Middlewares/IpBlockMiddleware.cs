using eXtensionSharp;
using eXtensionSharp.AspNet;
using Jennifer.Infrastructure.Abstractions.Behaviors;
using Jennifer.SharedKernel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Jennifer.Infrastructure.Middlewares;

public class IpBlockMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<IpBlockMiddleware> _logger;
    private readonly IIpBlockTtlService _ipBlockTtlService;
    private readonly IIpBlockStaticService _ipBlockStaticService;

    public IpBlockMiddleware(RequestDelegate next, ILogger<IpBlockMiddleware> logger,
        IIpBlockTtlService ipBlockTtlService,
        IIpBlockStaticService ipBlockStaticService)
    {
        _next = next;
        _logger = logger;
        _ipBlockTtlService = ipBlockTtlService;
        _ipBlockStaticService = ipBlockStaticService;
    }

    public async Task Invoke(HttpContext context)
    {
        ProblemDetails problemDetails = null;
        
        var ip = context.xGetRemoteIpAddress();
        if (ip.xIsEmpty())
        {
            problemDetails = new ProblemDetails
            {
                Title = "Unexpected error",
                Detail = "Unable to determine client ip",
                Status = StatusCodes.Status500InternalServerError,
                Instance = context.Request.Path
            };       
        }
        
        var @checked = await _ipBlockTtlService.IsBlockedAsync(ip);
        if (@checked)
        {
            problemDetails = new ProblemDetails
            {
                Title = "Unexpected error",
                Detail = "IP is blocked",
                Status = StatusCodes.Status500InternalServerError,
                Instance = context.Request.Path
            };
        }

        var staticChecked = await _ipBlockStaticService.IsBlocked(ip);
        if (staticChecked)
        {
            problemDetails = new ProblemDetails
            {
                Title = "Unexpected error",
                Detail = "IP is blocked",
                Status = StatusCodes.Status500InternalServerError,
                Instance = context.Request.Path
            };
        }

        if (problemDetails.xIsNotEmpty())
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(problemDetails);
            
            return;
        }
        
        await _next(context);
    }
}