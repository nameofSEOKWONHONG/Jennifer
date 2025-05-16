using Jennifer.Jwt.Models.Contracts;
namespace Jennifer.Jwt.Models;

public class EmailVerificationCode
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    /// <summary>
    /// 6자리 코드
    /// </summary>
    public string Code { get; set; }
    public ENUM_EMAIL_VERIFICATION_TYPE Type { get; set; }
    public int FailedCount { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    public bool IsExpired { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

