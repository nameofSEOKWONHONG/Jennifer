using eXtensionSharp.Mongo;

namespace Jennifer.External.OAuth.Abstracts;

public abstract class ExternalOAuthHandler: IExternalOAuthHandler
{
    public string Provider { get; private set; }
    protected readonly IHttpClientFactory httpClientFactory;
    protected readonly IJMongoFactory mongoFactory;
    protected ExternalOAuthHandler(IHttpClientFactory httpClientFactory, string provider)
    {
        this.httpClientFactory = httpClientFactory;   
        this.Provider = provider;       
    }

    protected ExternalOAuthHandler(IHttpClientFactory httpClientFactory, IJMongoFactory factory, string provider) :
        this(httpClientFactory, provider)
    {
        this.httpClientFactory = httpClientFactory;
        this.mongoFactory = factory;
        this.Provider = provider;
    }

    
    public abstract Task<IExternalOAuthResult> Verify(string providerToken, CancellationToken ct);
}