namespace Jennifer.External.OAuth.Abstracts;

public abstract class ExternalOAuthHandler: IExternalOAuthHandler
{
    public string Provider { get; private set; }
    protected readonly IHttpClientFactory _httpClientFactory;
    protected ExternalOAuthHandler(IHttpClientFactory httpClientFactory, string provider)
    {
        this._httpClientFactory = httpClientFactory;   
        this.Provider = provider;       
    }

    
    public abstract Task<IExternalOAuthResult> Verify(string providerToken, CancellationToken ct);
}