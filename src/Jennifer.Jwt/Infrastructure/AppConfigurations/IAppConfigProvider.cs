namespace Jennifer.SharedKernel.Infrastructure.AppConfigurations;

public interface IAppConfigProvider
{
    Task<string> GetAsync(string key);
    Task<IDictionary<string, string>> GetAllAsync(string prefix = null);
}
