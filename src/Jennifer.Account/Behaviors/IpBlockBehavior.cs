using eXtensionSharp;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.AspNetCore.Http;
using eXtensionSharp.AspNet;
using Jennifer.Account.Services;

namespace Jennifer.Account.Behaviors;

public class IpBlockBehavior<TMessage, TResponse>(IHttpContextAccessor accessor, IpBlockService service) : IPipelineBehavior<TMessage, TResponse>
    where TMessage : IMessage
    where TResponse : Result, new()
{
    public async ValueTask<TResponse> Handle(TMessage message, MessageHandlerDelegate<TMessage, TResponse> next, CancellationToken cancellationToken)
    {
        var ip = accessor.HttpContext.xGetRemoteIpAddress();
        if (ip.xIsEmpty())
            return new TResponse { IsSuccess = false, Message = "Unable to determine client ip" };
        
        var @checked = await service.IsBlockedAsync(ip);
        if (@checked)
            return new TResponse { IsSuccess = false, Message = "IP is blocked" };
        
        return await next(message, cancellationToken);
    }
}