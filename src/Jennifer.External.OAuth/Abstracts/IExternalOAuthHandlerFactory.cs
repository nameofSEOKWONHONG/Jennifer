namespace Jennifer.External.OAuth.Abstracts;

public interface IExternalOAuthHandlerFactory
{
    IExternalOAuthHandler Resolve(string providerName);
}