using Jennifer.SharedKernel;
using Mediator;

namespace Jennifer.Account.Application.Auth.Commands.CheckEmail;

/// <summary>
/// Request model for checking if an email already exists
/// </summary>
public sealed record CheckByEmailRequest(string Email);

/// <summary>
/// Query to check if an email already exists
/// Returns a Result indicating whether the email is available or already taken
/// </summary>
public sealed record CheckByEmailQuery(string Email): IRequest<Result>;