using System.Linq.Expressions;
using Jennifer.Infrastructure.Abstractions.Messaging;
using Jennifer.Jwt.Application.Auth.Contracts;
using Jennifer.Jwt.Models;
using LinqKit;

namespace Jennifer.Jwt.Application.Users.Commands;

public sealed record GetUserQuery(Guid UserId) : IQuery<UserDto>;