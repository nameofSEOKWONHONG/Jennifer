using Microsoft.Extensions.Caching.Memory;
using StackExchange.Redis;

namespace Jennifer.Infrastructure.Abstractions.Behaviors;

public interface IIpBlockService
{
    Task BlockIpAsync(string ip);

    Task UnblockIpAsync(string ip);

    Task<bool> IsBlockedAsync(string ip);

    void SubscribeToUpdates();
}

public class IpBlockService: IIpBlockService
{
    private readonly IMemoryCache _cache;
    private readonly IDatabase _redis;
    private readonly ISubscriber _subscriber;
    private static readonly TimeSpan _ttl = TimeSpan.FromMinutes(10);

    
    public IpBlockService(IConnectionMultiplexer redis, IMemoryCache cache)
    {
        _cache = cache;
        _redis = redis.GetDatabase();
        _subscriber = redis.GetSubscriber();
    }

    public async Task BlockIpAsync(string ip)
    {
        var key = $"ip:block:{ip}";
        await _redis.StringSetAsync(key, "1");
        _cache.Set(key, true, _ttl);
        
        await _subscriber.PublishAsync(RedisChannel.Literal("ip:block:update"), ip);
    }
    
    public async Task UnblockIpAsync(string ip)
    {
        var key = $"ip:block:{ip}";
        await _redis.KeyDeleteAsync(key);
        _cache.Remove(key);
        
        await _redis.KeyDeleteAsync(key);
    }

    public async Task<bool> IsBlockedAsync(string ip)
    {
        var key = $"ip:block:{ip}";

        if (_cache.TryGetValue(key, out bool cached) && cached)
            return true;

        var exists = await _redis.KeyExistsAsync(key);
        if (exists)
            _cache.Set(key, true, _ttl);

        return exists;
    }

    public void SubscribeToUpdates()
    {
        _subscriber.Subscribe(RedisChannel.Literal("ip:block:update"), async (channel, message) =>
        {
            var parts = message.ToString().Split('§');
            if (parts.Length != 2) return;

            var action = parts[0];
            var ip = parts[1];

            switch (action)
            {
                case "add":
                    await BlockIpAsync(ip); // 기본 TTL 예시
                    break;

                case "remove":
                    await UnblockIpAsync(ip);
                    break;
            }
        });
    }
}
