using Asp.Versioning;
using Jennifer.Account.Application.Menus.Commands;
using Jennifer.Account.Application.Menus.Queries;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Jennifer.Account.Application.Menus;

public static class MenuEndpoint
{
    public static void MapMenuEndpoint(this IEndpointRouteBuilder endpoint)
    {
        var apiVersionSet = endpoint.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .ReportApiVersions()
            .Build();
        
        var group = endpoint.MapGroup("/api/v{version:apiVersion}/menu")
                .WithTags("Menu")
                .WithApiVersionSet(apiVersionSet)
                .RequireAuthorization()
            ;
        
        group.MapGet("/", async (ISender sender, CancellationToken ct) =>
            await sender.Send(new GetsMenuQuery(), ct))
            .MapToApiVersion(1)
            .WithName("GetMenus")
            .WithDescription("메뉴 목록 조회");
        
        group.MapPost("/", async ([FromBody]MenuDto item, ISender sender, CancellationToken ct) =>
            await sender.Send(new CreateMenuCommand(item), ct))
            .MapToApiVersion(1)
            .WithName("CreateMenu")
            .WithDescription("메뉴 생성");
        
        group.MapDelete("/{id}", async(Guid id, ISender sender, CancellationToken ct) =>
            await sender.Send(new RemoveMenuCommand(id), ct))
            .MapToApiVersion(1)
            .WithName("RemoveMenu")
            .WithDescription("메뉴 삭제");
        
        group.MapPatch("/", async ([FromBody]MenuDto dto, ISender sender, CancellationToken ct) =>
            await sender.Send(new ModifyMenuCommand(dto)))
            .MapToApiVersion(1)
            .WithName("ModifyMenu")
            .WithDescription("메뉴 수정");   
    }
}