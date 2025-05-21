using Jennifer.Infrastructure.Abstractions;
using Jennifer.Jwt.Data;
using Jennifer.Jwt.Models.Contracts;
using Jennifer.Jwt.Session;
using Jennifer.Jwt.Session.Abstracts;
using Jennifer.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Jennifer.Jwt.Application.Auth.Services.Implements;

public sealed record ConfigurationResponse(ENUM_CONFIGURATION_TYPE Type, string Value);

public interface IGetConfigurationService : IServiceBase<ENUM_CONFIGURATION_TYPE, IEnumerable<ConfigurationResponse>>
{
    
}

public class ConfigurationGetService: SessionServiceBase<ConfigurationGetService, ENUM_CONFIGURATION_TYPE, ConfigurationResponse>, IGetConfigurationService
{
    private readonly JenniferDbContext _dbContext;

    public ConfigurationGetService(ILogger<ConfigurationGetService> logger, ISessionContext sessionContext,
        JenniferDbContext dbContext) : base(logger, sessionContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<ConfigurationResponse>> HandleAsync(ENUM_CONFIGURATION_TYPE request, CancellationToken cancellationToken) =>
        await _dbContext.Configurations
            .AsNoTracking()
            .Where(m => m.Type == request)
            .Select(m => new ConfigurationResponse(m.Type, m.Value))
            .ToArrayAsync(cancellationToken: cancellationToken);
}