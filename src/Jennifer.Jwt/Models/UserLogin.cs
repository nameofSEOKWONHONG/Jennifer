using Microsoft.AspNetCore.Identity;

namespace Jennifer.Jwt.Models;

public class UserLogin : IdentityUserLogin<Guid>
{
    public User User { get; set; }
}

