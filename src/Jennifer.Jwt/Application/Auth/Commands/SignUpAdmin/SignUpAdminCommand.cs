using Jennifer.Jwt.Abstractions.Messaging;

namespace Jennifer.Jwt.Application.Auth.Commands.SignUpAdmin;

public sealed record SignUpAdminCommand(string Email, string Password)
    : ICommand<Guid>;