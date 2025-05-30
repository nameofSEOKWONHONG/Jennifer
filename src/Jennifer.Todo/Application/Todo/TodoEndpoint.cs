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
            .WithName("GetsTodo")
            .WithDescription("");
        
        group.MapGet("/{id}", async ([AsParameters]GetTodoRequest request, ISender sender, CancellationToken cancellationToken) =>
            await sender.Send(new GetTodoQuery(request.Id), cancellationToken))
            .WithName("GetTodo")
            .WithDescription("");
        
        group.MapPost("/", async ([FromBody]TodoItemDto dto, ISender sender, CancellationToken cancellationToken) =>
            await sender.Send(new CreateTodoCommand(dto), cancellationToken))
            .WithName("CreateTodo")
            .WithDescription("");
        
        group.MapPut("/", async ([FromBody]TodoItemDto dto, ISender sender, CancellationToken cancellationToken) =>
            await sender.Send(new UpdateTodoCommand(dto), cancellationToken))
            .WithName("UpdateTodo")
            .WithDescription("");
        
        group.MapDelete("/{id}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
            await sender.Send(new RemoveTodoItemCommand(id), cancellationToken))
            .WithName("RemoveTodo")
            .WithDescription("");
    }
}