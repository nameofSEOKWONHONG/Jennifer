using Jennifer.SharedKernel;
using Jennifer.SharedKernel.Account.Auth;
using Mediator;

namespace Jennifer.Account.Application.Users.Queries;

public sealed record GetUserQuery(Guid UserId) : IQuery<Result<UserDto>>;