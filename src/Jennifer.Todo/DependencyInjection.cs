using Grpc.Net.Client;
using Jennifer.Account.Grpc;
using Jennifer.Todo.Application.Todo;
using Jennifer.Todo.Application.Todo.Queries;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Jennifer.Todo;

public static class DependencyInjection
{
    public static void AddTodo(this IServiceCollection services)
    {
        services.AddScoped<ITodoQueryFilter, TodoQueryFilter>();
        services.AddSingleton(provider =>
        {
            var channel = GrpcChannel.ForAddress("https://localhost:7288", new GrpcChannelOptions
            {
                HttpHandler = new SocketsHttpHandler
                {
                    PooledConnectionIdleTimeout = TimeSpan.FromMinutes(5),
                    KeepAlivePingDelay = TimeSpan.FromSeconds(30),
                    KeepAlivePingTimeout = TimeSpan.FromSeconds(10),
                    EnableMultipleHttp2Connections = true
                }
            });

            return new AccountService.AccountServiceClient(channel);
        });
    }

    public static void UseTodo(this IEndpointRouteBuilder app)
    {
        app.MapTodoEndpoint();
    }
}