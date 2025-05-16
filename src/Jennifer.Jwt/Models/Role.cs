using Microsoft.AspNetCore.Identity;

namespace Jennifer.Jwt.Models;

public class Role: IdentityRole<Guid>
{
    public virtual ICollection<UserRole> UserRoles { get; set; }
    public virtual ICollection<RoleClaim> RoleClaims { get; set; }
    

}