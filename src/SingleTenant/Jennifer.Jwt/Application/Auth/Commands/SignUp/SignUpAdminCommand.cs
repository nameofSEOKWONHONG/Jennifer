using Jennifer.SharedKernel;
using Mediator;

namespace Jennifer.Jwt.Application.Auth.Commands.SignUp;

public sealed record SignUpAdminCommand(string Email, string Password, string UserName, string PhoneNumber)
    : ICommand<Result<Guid>>;