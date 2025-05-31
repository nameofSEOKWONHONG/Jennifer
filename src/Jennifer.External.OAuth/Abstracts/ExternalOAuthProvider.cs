using eXtensionSharp.Mongo;

namespace Jennifer.External.OAuth.Abstracts;

public abstract class ExternalOAuthProvider: IExternalOAuthProvider
{
    public string Provider { get; private set; }
    protected readonly IHttpClientFactory httpClientFactory;
    protected readonly IJMongoFactory mongoFactory;
    protected ExternalOAuthProvider(IHttpClientFactory httpClientFactory, string provider)
    {
        this.httpClientFactory = httpClientFactory;   
        this.Provider = provider;       
    }

    protected ExternalOAuthProvider(IHttpClientFactory httpClientFactory, IJMongoFactory factory, string provider) :
        this(httpClientFactory, provider)
    {
        this.httpClientFactory = httpClientFactory;
        this.mongoFactory = factory;
        this.Provider = provider;
    }

    
    public abstract Task<IExternalOAuthResult> AuthenticateAsync(string providerToken, CancellationToken ct);
}