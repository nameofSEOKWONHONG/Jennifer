using System.Collections.Concurrent;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace Jennifer.Jwt.Hubs;

public class JenniferHub: Hub
{
    public JenniferHub()
    {
        
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier!;
        var newConnectionId = Context.ConnectionId;

        var existingConnections = ConnectionStore.Get(userId);
        
        foreach (var connId in existingConnections)
        {
            // 기존 연결된 사용자에게 "다른 데서 로그인 시도됨" 알림 전송
            await Clients.Client(connId).SendAsync("NotifyNewLogin", connId);
        }
        
        // 새 연결도 등록
        ConnectionStore.Add(userId, newConnectionId);
        
        await base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        ConnectionStore.RemoveByConnectionId(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }

    public async Task RejectNewLogin(string connectionIdToKick)
    {
        await Clients.Client(connectionIdToKick)
            .SendAsync("ForceLogout", "기존 사용자에 의해 접속이 차단되었습니다.");

        // 선택적으로 연결 관리 테이블에서 제거
        ConnectionStore.RemoveByConnectionId(connectionIdToKick);
    }
}

public class ConnectionStore
{
    // Key: UserId (string), Value: List of ConnectionIds
    private static readonly ConcurrentDictionary<string, HashSet<string>> _connections = new();

    public static void Add(string userId, string connectionId)
    {
        _connections.AddOrUpdate(userId,
            _ => new HashSet<string> { connectionId },
            (_, existingSet) =>
            {
                lock (existingSet)
                {
                    existingSet.Add(connectionId);
                    return existingSet;
                }
            });
    }

    public static void RemoveByConnectionId(string connectionId)
    {
        foreach (var kvp in _connections)
        {
            var userId = kvp.Key;
            var set = kvp.Value;

            lock (set)
            {
                if (set.Remove(connectionId) && set.Count == 0)
                {
                    _connections.TryRemove(userId, out _);
                }
            }
        }
    }

    public static void RemoveAllByUserId(string userId)
    {
        _connections.TryRemove(userId, out _);
    }

    public static IEnumerable<string> Get(string userId)
    {
        return _connections.TryGetValue(userId, out var set) ? set.ToList() : Enumerable.Empty<string>();
    }

    public static bool Contains(string userId, string connectionId)
    {
        return _connections.TryGetValue(userId, out var set) && set.Contains(connectionId);
    }

    public static string GetLatestConnectionId(string userId)
    {
        return _connections.TryGetValue(userId, out var set)
            ? set.LastOrDefault()
            : null;
    }
}

internal class UserIdProvider : IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
    {
        return connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}