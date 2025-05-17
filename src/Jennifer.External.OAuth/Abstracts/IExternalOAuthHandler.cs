namespace Jennifer.External.OAuth.Abstracts;

public interface IExternalOAuthHandler
{
    string Provider { get; } // "facebook", "google" 등
    Task<IExternalOAuthResult> Verify(string providerToken, CancellationToken ct);

}