using Asp.Versioning;
using Jennifer.Account.Models.Contracts;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Jennifer.Account.Application.Options;

internal static class OptionEndpoint
{
    internal static void MapOptionEndpoint(this IEndpointRouteBuilder endpoints)
    {
        var apiVersionSet = endpoints.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .ReportApiVersions()
            .Build();
        
        var group = endpoints.MapGroup("/api/v{version:apiVersion}/option")
                .WithTags("Option")
                .WithApiVersionSet(apiVersionSet)
                .RequireAuthorization()
            ;
        group.MapGet("/{type}/{pageNo}/{pageSize}", async (string type, int pageNo, int pageSize,
                    ISender sender, CancellationToken cancellationToken) =>
                await sender.Send(new GetsOptionQuery(ENUM_ACCOUNT_OPTION.FromValue(type), pageNo, pageSize), cancellationToken))
            .MapToApiVersion(1)
            .WithName("GetsOptions")
            .WithDescription("유형으로 필터링된 옵션 목록 페이징 조회");
        
        group.MapPost("/", async (CreateOptionCommand request, ISender sender, CancellationToken cancellationToken) =>
                await sender.Send(request, cancellationToken))
            .MapToApiVersion(1)
            .WithName("AddOption") 
            .WithDescription("새로운 옵션을 생성");
        
        group.MapPatch("/", async ([FromBody]UpdateOptionCommand request, ISender sender, CancellationToken cancellationToken) =>
                await sender.Send(request, cancellationToken))
            .MapToApiVersion(1)
            .WithName("ModifyOption")
            .WithDescription("기존 옵션을 수정");
        
        group.MapDelete("/{id}", async (int id, ISender sender, CancellationToken cancellationToken) =>
                await sender.Send(new RemoveOptionCommand(id), cancellationToken))
            .MapToApiVersion(1)
            .WithName("RemoveOption")
            .WithDescription("아이디로 옵션을 삭제");
    }
}