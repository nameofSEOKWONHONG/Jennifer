using Asp.Versioning;
using Jennifer.Todo.Application.Todo.Commands;
using Jennifer.Todo.Application.Todo.Contracts;
using Jennifer.Todo.Application.Todo.Queries;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Jennifer.Todo.Application.Todo;

public static class Endpoint
{
    public static void MapTodoEndpoint(this IEndpointRouteBuilder endpoint)
    {
        var apiVersionSet = endpoint.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .ReportApiVersions()
            .Build();
        
        var group = endpoint.MapGroup("/api/v{version:apiVersion}/todo")
                .WithTags("Todo")
                .WithApiVersionSet(apiVersionSet)
                .RequireAuthorization()
            ;

        group.MapGet("/", async ([AsParameters]GetsTodoRequest request, ISender sender, CancellationToken cancellationToken) =>
                await sender.Send(new GetsTodoQuery(request.Description, request.PageNo, request.PageSize),
                    cancellationToken))
                .MapToApiVersion(1)
            .WithName("GetsTodo")
            .WithDescription("Gets a paginated list of todo items optionally filtered by description");
        
        group.MapGet("/{id}", async ([AsParameters]GetTodoRequest request, ISender sender, CancellationToken cancellationToken) =>
            await sender.Send(new GetTodoQuery(request.Id), cancellationToken))
            .MapToApiVersion(1)
            .WithName("GetTodo")
            .WithDescription("Gets a specific todo item by ID");
        
        group.MapPost("/", async ([FromBody]TodoItemDto dto, ISender sender, CancellationToken cancellationToken) =>
            await sender.Send(new CreateTodoItemCommand(dto), cancellationToken))
            .MapToApiVersion(1)
            .WithName("CreateTodo") 
            .WithDescription("Creates a new todo item");
        
        group.MapPut("/", async ([FromBody]TodoItemDto dto, ISender sender, CancellationToken cancellationToken) =>
            await sender.Send(new UpdateTodoItemCommand(dto), cancellationToken))
            .MapToApiVersion(1)
            .WithName("UpdateTodo")
            .WithDescription("Updates an existing todo item");
        
        group.MapDelete("/{id}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
            await sender.Send(new RemoveTodoItemCommand(id), cancellationToken))
            .MapToApiVersion(1)
            .WithName("RemoveTodo")
            .WithDescription("Removes a todo item by ID");
    }
}