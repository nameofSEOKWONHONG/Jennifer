using Jennifer.Infrastructure.AppConfigurations;
using StackExchange.Redis;

namespace Jennifer.Infrastructure.Middlewares;

public interface IIpBlockTtlService
{
    Task BlockIpAsync(string ip);

    Task UnblockIpAsync(string ip);

    Task<bool> IsBlockedAsync(string ip);

    void SubscribeToUpdates();
}

public class IpBlockTtlService: IIpBlockTtlService
{
    private readonly IDatabase _redis;
    private readonly ISubscriber _subscriber;
    private static readonly TimeSpan _ttl = TimeSpan.FromMinutes(10);

    public IpBlockTtlService(IConnectionMultiplexer redis)
    {
        _redis = redis.GetDatabase();
        _subscriber = redis.GetSubscriber();
    }
    
    public async Task BlockIpAsync(string ip)
    {
        var key = $"ip:block:{ip}";
        await _redis.StringSetAsync(key, "1", _ttl, When.NotExists);
        
        await _subscriber.PublishAsync(RedisChannel.Literal("ip:block:update"), ip);
    }
    
    public async Task UnblockIpAsync(string ip)
    {
        var key = $"ip:block:{ip}";
        await _redis.KeyDeleteAsync(key);
        
        await _redis.KeyDeleteAsync(key);
    }

    public async Task<bool> IsBlockedAsync(string ip)
    {
        if (!WithOptions.Instance.WorkIpBlock) return true;
        
        var key = $"ip:block:{ip}";
        
        var exists = await _redis.KeyExistsAsync(key);
        
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
