using eXtensionSharp;
using Jennifer.Account.Data;
using Jennifer.Account.Models;
using Jennifer.Account.Models.Contracts;
using Jennifer.Account.Session;
using Jennifer.Account.Session.Abstracts;
using Jennifer.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Jennifer.Account.Application.Auth.Services.Implements;

public sealed record ConfigurationAddRequest(Guid Id, ENUM_CONFIGURATION_TYPE Type, string Value);

public interface IConfigurationAddService : IServiceBase<ConfigurationAddRequest, ServiceResult<Guid>>;

internal sealed class ConfigurationAddService: SessionServiceBase<ConfigurationAddService, ConfigurationAddRequest, ServiceResult<Guid>>, IConfigurationAddService
{
    private readonly JenniferDbContext _dbContext;

    public ConfigurationAddService(ILogger<ConfigurationAddService> logger, ISessionContext sessionContext,
        JenniferDbContext dbContext) : base(logger, sessionContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ServiceResult<Guid>> HandleAsync(ConfigurationAddRequest request, CancellationToken cancellationToken)
    {
        var exists = _dbContext.Configurations
            .FirstOrDefaultAsync(m => m.Id == request.Id && m.Type == request.Type, cancellationToken: cancellationToken);
        if (exists.xIsEmpty()) return ServiceResult<Guid>.Failure("Not found");

        var newItem = new Configuration
        {
            Type = request.Type,
            Value = request.Value
        }; 
        await _dbContext.Configurations.AddAsync(newItem, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ServiceResult<Guid>.Success(newItem.Id) ;
    }
}

