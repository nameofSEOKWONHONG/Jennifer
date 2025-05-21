using Jennifer.Jwt.Application.Auth.Contracts;
using Jennifer.SharedKernel;
using Mediator;

namespace Jennifer.Jwt.Application.Users.Commands;

public sealed record GetUserQuery(Guid UserId) : IQuery<Result<UserDto>>;