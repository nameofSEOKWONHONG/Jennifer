using Jennifer.SharedKernel.Consts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jennifer.Jwt.Models;
public class RoleClaim: IdentityRoleClaim<Guid>
{
    public Role Role { get; set; }

}