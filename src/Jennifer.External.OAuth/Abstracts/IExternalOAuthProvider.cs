namespace Jennifer.External.OAuth.Abstracts;

public interface IExternalOAuthProvider
{
    string Provider { get; } // "facebook", "google" 등
    Task<IExternalOAuthResult> AuthenticateAsync(string providerToken, CancellationToken ct);

}