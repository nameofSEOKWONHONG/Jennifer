namespace Jennifer.SharedKernel.Account.Menu;

public sealed record MenuDto(Guid Id,
    string Name,
    string Icon,
    string Url,
    Guid? ParentId,
    int Order,
    bool IsVisible,
    List<MenuDto> Children);