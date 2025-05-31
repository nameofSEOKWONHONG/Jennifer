namespace Jennifer.External.OAuth.Abstracts;

public interface IExternalOAuthProviderFactory
{
    IExternalOAuthProvider Resolve(string providerName);
}