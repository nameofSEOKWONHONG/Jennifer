namespace Jennifer.External.OAuth.Contracts;

public class GoogleOAuthResult
{
    public string OId { get; set; }
    public string Sub { get; set; }
    public string Name { get; set; }
    public string GivenName { get; set; }
    public string FamilyName { get; set; }
    public string Picture { get; set; }
    public string Email { get; set; }
    public bool EmailVerified { get; set; }
    public string Locale { get; set; }
    public string ProviderId => Sub;
}

public class AppleOAuthResult
{
    public string ProviderId { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
}