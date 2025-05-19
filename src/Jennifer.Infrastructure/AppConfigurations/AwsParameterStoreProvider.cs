using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;

namespace Jennifer.Infrastructure.AppConfigurations;

public class AwsParameterStoreProvider : IAppConfigProvider
{
    private readonly IAmazonSimpleSystemsManagement _ssm;
    private readonly string _basePath;

    public AwsParameterStoreProvider(IAmazonSimpleSystemsManagement ssm, string basePath)
    {
        _ssm = ssm;
        _basePath = basePath;
    }

    public async Task<string> GetAsync(string key)
    {
        var name = $"{_basePath}/{key.Replace(":", "/")}";
        var response = await _ssm.GetParameterAsync(new GetParameterRequest
        {
            Name = name,
            WithDecryption = true
        });
        return response.Parameter?.Value;
    }

    public async Task<IDictionary<string, string>> GetAllAsync(string prefix = null)
    {
        var result = new Dictionary<string, string>();
        string nextToken = null;

        do
        {
            var request = new GetParametersByPathRequest
            {
                Path = $"{_basePath}/{prefix?.Replace(":", "/") ?? ""}",
                Recursive = true,
                WithDecryption = true,
                NextToken = nextToken
            };

            var response = await _ssm.GetParametersByPathAsync(request);
            foreach (var param in response.Parameters)
            {
                var key = param.Name.Replace(_basePath + "/", "").Replace("/", ":");
                result[key] = param.Value;
            }

            nextToken = response.NextToken;
        } while (!string.IsNullOrEmpty(nextToken));

        return result;
    }
}
