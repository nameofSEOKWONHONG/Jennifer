using Jennifer.SharedKernel;
using Mediator;

namespace Jennifer.Account.Application.Menus.Queries;

public sealed record MenuDto(Guid Id,
    string Name,
    string Icon,
    string Url,
    Guid? ParentId,
    int Order,
    bool IsVisible,
    List<MenuDto> Children);

public sealed record GetsMenuQuery: IQuery<Result<MenuDto[]>>;
