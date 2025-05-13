namespace Jennifer.Core.Domains;

public interface IExternalSignResult
{
    string ProviderId { get; }
    string Email { get; }
    string Name { get; }
}

public class GoogleSignResult: IExternalSignResult
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

public class AppleSignResult : IExternalSignResult
{
    public string ProviderId { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
}