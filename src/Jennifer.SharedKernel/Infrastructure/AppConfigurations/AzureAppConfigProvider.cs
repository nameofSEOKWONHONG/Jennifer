using Microsoft.Extensions.Configuration;

namespace Jennifer.SharedKernel.Infrastructure.AppConfigurations;

public class AzureAppConfigProvider : IAppConfigProvider
{
    private readonly IConfiguration _configuration;

    public AzureAppConfigProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<string?> GetAsync(string key)
    {
        return Task.FromResult(_configuration[key]);
    }

    public Task<IDictionary<string, string>> GetAllAsync(string? prefix = null)
    {
        var values = _configuration
            .AsEnumerable()
            .Where(kv => prefix == null || kv.Key.StartsWith(prefix))
            .ToDictionary(kv => kv.Key, kv => kv.Value ?? "");
        return Task.FromResult<IDictionary<string, string>>(values);
    }
}
