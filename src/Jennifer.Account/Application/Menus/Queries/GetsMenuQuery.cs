using Jennifer.SharedKernel;
using Jennifer.SharedKernel.Account.Menu;
using Mediator;

namespace Jennifer.Account.Application.Menus.Queries;

public sealed record GetsMenuQuery: IQuery<Result<MenuDto[]>>;
