namespace Jennifer.Core.Domains;

public class PasswordResetRequest
{
    public string ResetToken { get; set; }
    public string Password { get; set; }
    public string NewPassword { get; set; }
}