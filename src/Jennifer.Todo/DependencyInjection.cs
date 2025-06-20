using Grpc.Net.Client;
using Jennifer.Account.Grpc;
using Jennifer.Infrastructure.Database;
using Jennifer.Todo.Application.Todo;
using Jennifer.Todo.Application.Todo.Queries;
using Jennifer.Todo.GrpcServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Jennifer.Todo;

public static class DependencyInjection
{
    public static void AddTodo(this IServiceCollection services,
        Action<IServiceProvider, DbContextOptionsBuilder> dbContextSetup
        )
    {
        services.AddDbContext<TodoDbContext>(dbContextSetup);
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
        app.MapGrpcService<TodoServiceImpl>();
    }
}