using Jennifer.SharedKernel.Domains;

namespace Jennifer.SharedKernel.Infrastructure.SignHandlers;

public abstract class ExternalSignHandler
{
    protected readonly IHttpClientFactory _httpClientFactory;
    protected ExternalSignHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;   
    }
    
    public abstract Task<IExternalSignResult> Verify(string providerToken, CancellationToken ct);
}