using Jennifer.SharedKernel;

namespace Jennifer.Jwt.Application.Auth.Commands.SignUpAdmin;

public sealed record SignUpAdminDomainEvent(Guid UserId) : IDomainEvent;
