using Asp.Versioning;
using Jennifer.Account.Application.Options.Commands;
using Jennifer.Account.Application.Options.Queries;
using Jennifer.Domain.Accounts.Contracts;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Jennifer.Account.Application.Options;

public static class OptionEndpoint
{
    public static void MapOptionEndpoint(this IEndpointRouteBuilder endpoints)
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
                await sender.Send(new GetsOptionQuery(ENUM_OPTION_TYPE.FromValue(type), pageNo, pageSize), cancellationToken))
            .MapToApiVersion(1)
            .WithName("GetsOptions")
            .WithDescription("유형으로 필터링된 옵션 목록 페이징 조회");
        
        group.MapPost("/", async (CreateOptionRequest request, ISender sender, CancellationToken cancellationToken) =>
                await sender.Send(new CreateOptionCommand(request.Type, request.Value), cancellationToken))
            .MapToApiVersion(1)
            .WithName("AddOption") 
            .WithDescription("새로운 옵션을 생성");
        
        group.MapPatch("/", async ([FromBody]UpdateOptionRequest request, ISender sender, CancellationToken cancellationToken) =>
                await sender.Send(new UpdateOptionCommand(request.Id, request.Type, request.Value), cancellationToken))
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