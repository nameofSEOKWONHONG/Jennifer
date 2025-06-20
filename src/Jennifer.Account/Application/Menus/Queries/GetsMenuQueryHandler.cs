using Jennifer.Infrastructure.Database;
using Jennifer.SharedKernel;
using Jennifer.SharedKernel.Account.Menu;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;

namespace Jennifer.Account.Application.Menus.Queries;

public sealed class GetsMenuQueryHandler(
    JenniferDbContext dbContext,
    HybridCache cache
):IQueryHandler<GetsMenuQuery, Result<MenuDto[]>>
{
    public async ValueTask<Result<MenuDto[]>> Handle(GetsMenuQuery query, CancellationToken cancellationToken)
    {
        async ValueTask<MenuDto[]> FetchFromDatabase(CancellationToken token) =>
            await dbContext.Menus
                .AsNoTracking()
                .Where(m => m.ParentId == null) // 루트 메뉴만 조회
                .Include(m => m.Children)       // 레벨 2까지 포함
                .Select(m => new MenuDto(
                    m.Id,
                    m.Name,
                    m.Icon,
                    m.Url,
                    m.ParentId,
                    m.Order,
                    m.IsVisible,
                    m.Children
                        .OrderBy(c => c.Order)
                        .Select(c => new MenuDto(
                            c.Id,
                            c.Name,
                            c.Icon,
                            c.Url,
                            c.ParentId,
                            c.Order,
                            c.IsVisible,
                            new List<MenuDto>() // 레벨 2까지만 지원
                        )).ToList()
                ))
                .OrderBy(m => m.Order)
                .ToArrayAsync(token);

        var result = await cache.GetOrCreateAsync("menu", FetchFromDatabase, new HybridCacheEntryOptions()
        {
            Expiration = TimeSpan.FromMinutes(5),
            LocalCacheExpiration = TimeSpan.FromMinutes(5)
        }, cancellationToken: cancellationToken);
        return await Result<MenuDto[]>.SuccessAsync(result);
    }
}