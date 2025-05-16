using Microsoft.AspNetCore.Identity;

namespace Jennifer.Jwt.Models;

public class UserClaim : IdentityUserClaim<Guid>
{
    public User User { get; set; }
    public string Source { get; set; } // 예: 클레임의 출처
}