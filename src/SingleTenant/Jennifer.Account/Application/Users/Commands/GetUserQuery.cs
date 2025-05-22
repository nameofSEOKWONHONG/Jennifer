using Jennifer.Account.Application.Auth.Contracts;
using Jennifer.SharedKernel;
using Mediator;

namespace Jennifer.Account.Application.Users.Commands;

public sealed record GetUserQuery(Guid UserId) : IQuery<Result<UserDto>>;