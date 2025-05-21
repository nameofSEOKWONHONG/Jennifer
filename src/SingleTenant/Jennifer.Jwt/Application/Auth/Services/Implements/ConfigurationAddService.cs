using eXtensionSharp;
using Jennifer.Infrastructure.Abstractions;
using Jennifer.Jwt.Data;
using Jennifer.Jwt.Models;
using Jennifer.Jwt.Models.Contracts;
using Jennifer.Jwt.Session;
using Jennifer.Jwt.Session.Abstracts;
using Jennifer.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Jennifer.Jwt.Application.Auth.Services.Implements;

public sealed record ConfigurationAddRequest(Guid Id, ENUM_CONFIGURATION_TYPE Type, string Value);

public interface IConfigurationAddService : IServiceBase<ConfigurationAddRequest, ServiceResult<Guid>>;

public class ConfigurationAddService: SessionServiceBase<ConfigurationAddService, ConfigurationAddRequest, ServiceResult<Guid>>, IConfigurationAddService
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

public class ServiceResult<T>
{
    public T Data { get; set; }
    public string Message { get; set; }
    public bool IsSuccess { get; set; }
    
    public static ServiceResult<T> Success(T data) => new ServiceResult<T>
    {
        Data = data,
        IsSuccess = true
    };
    public static ServiceResult<T> Failure(string message) => new ServiceResult<T>
    {
        Message = message,
        IsSuccess = false
    };
}