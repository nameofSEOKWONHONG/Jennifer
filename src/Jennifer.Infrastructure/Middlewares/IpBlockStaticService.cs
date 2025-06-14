using System.Collections.Concurrent;
using Jennifer.Domain.Common;
using Jennifer.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Jennifer.Infrastructure.Middlewares;

public interface IIpBlockStaticService
{
    Task Setup();
    Task<bool> AddBlocked(string ip);
    Task<bool> UnblockIpAsync(string ip);
    Task<bool> IsBlocked(string ip);
}

public class IpBlockStaticService(IServiceScopeFactory serviceScopeFactory): IIpBlockStaticService
{
    private ConcurrentBag<string> _ipBag = new();
    
    public async Task Setup()
    {
        using var scope = serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<JenniferDbContext>();
        var ipList = await dbContext.IpBlockLogs.AsNoTracking()
            .Where(m => m.IsPermanent == true)
            .Select(m => m.IpAddress)
            .ToArrayAsync();
        
        _ipBag = new ConcurrentBag<string>(_ipBag);
    }

    public async Task<bool> AddBlocked(string ip)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<JenniferDbContext>();
        await dbContext.IpBlockLogs.AddAsync(new IpBlockLog()
        {
            IpAddress = ip,
            BlockedAt = DateTime.UtcNow,
            ExpiresAt = null, // 예: 1시간 임시 차단
            Reason = "Permanent block",
            CreatedBy = "SYSTEM",
            IsPermanent = true,
        });
        await dbContext.SaveChangesAsync();
        if (!_ipBag.Contains(ip))
        {
            _ipBag.Add(ip);
        }
        return true;
    }
    
    public async Task<bool> UnblockIpAsync(string ip)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<JenniferDbContext>();
        var exists = await dbContext.IpBlockLogs.FirstOrDefaultAsync(m => m.IpAddress == ip);
        dbContext.IpBlockLogs.Remove(exists);
        await dbContext.SaveChangesAsync();
        
        if (_ipBag.Contains(ip))
        {
            _ipBag.TryTake(out _);
        }

        return true;
    }

    public async Task<bool> IsBlocked(string ip)
    {
        if (_ipBag.Contains(ip))
        {
            return true;
        }
        
        using var scope = serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<JenniferDbContext>();
        var exists = await dbContext.IpBlockLogs.AnyAsync(m => m.IpAddress == ip);
        if (!exists) return false;
        
        if (!_ipBag.Contains(ip))
        {
            _ipBag.Add(ip);
        }
        return true;
    }
}

