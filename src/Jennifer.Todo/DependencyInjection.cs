using Jennifer.Todo.Application.Todo;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Jennifer.Todo;

public static class DependencyInjection
{
    public static void AddTodo(this IServiceCollection services)
    {
        
    }

    public static void UseTodo(this IEndpointRouteBuilder app)
    {
        app.MapTodoEndpoint();
    }
}