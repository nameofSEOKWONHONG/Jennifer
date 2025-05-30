using Jennifer.SharedKernel;
using Mediator;

namespace Jennifer.Account.Application.Auth.Commands.CheckEmail;

public sealed record CheckByEmailRequest(string Email);

public sealed record CheckByEmailQuery(string Email): IRequest<Result>;
