using Jennifer.Jwt.Models.Contracts;
using Jennifer.Jwt.Services.AuthServices.Contracts;
using Jennifer.SharedKernel.Domains;
using Jennifer.SharedKernel.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace Jennifer.Jwt.Models;

public class User : IdentityUser<Guid>, IAuditable
{
    public ENUM_USER_TYPE Type { get; set; }
    public bool IsDelete { get; set; }
    public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ModifiedOn { get; set; }
    public virtual ICollection<UserRole> UserRoles { get; set; }
    public virtual ICollection<UserClaim> Claims { get; set; }
    public virtual ICollection<UserLogin> Logins { get; set; }
    public virtual ICollection<UserToken> Tokens { get; set; }
}