using Jennifer.Account.Data;
using Jennifer.Account.Models.Contracts;
using Jennifer.Account.Session;
using Jennifer.Account.Session.Abstracts;
using Jennifer.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Jennifer.Account.Application.Auth.Services.Implements;

public sealed record ConfigurationResponse(ENUM_CONFIGURATION_TYPE Type, string Value);

public interface IGetConfigurationService : IServiceBase<ENUM_CONFIGURATION_TYPE, IEnumerable<ConfigurationResponse>>
{
    
}

public class ConfigurationGetService: SessionServiceBase<ConfigurationGetService, ENUM_CONFIGURATION_TYPE, ConfigurationResponse>, IGetConfigurationService
{
    public ConfigurationGetService(ILogger<ConfigurationGetService> logger, ISessionContext sessionContext) : base(logger, sessionContext)
    {
    }

    public async Task<IEnumerable<ConfigurationResponse>> HandleAsync(ENUM_CONFIGURATION_TYPE request, CancellationToken cancellationToken) =>
        await this.AsDatabase<JenniferDbContext>().Configurations
            .AsNoTracking()
            .Where(m => m.Type == request)
            .Select(m => new ConfigurationResponse(m.Type, m.Value))
            .ToArrayAsync(cancellationToken: cancellationToken);
}