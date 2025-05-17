using Jennifer.SharedKernel;

namespace Jennifer.Jwt.Application.SignUpAdmin;

public sealed record SignUpAdminDomainEvent(Guid UserId) : IDomainEvent;
