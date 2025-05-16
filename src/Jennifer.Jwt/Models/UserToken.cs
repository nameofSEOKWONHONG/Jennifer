using Microsoft.AspNetCore.Identity;

namespace Jennifer.Jwt.Models;

public class UserToken : IdentityUserToken<Guid>
{
    public User User { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
}