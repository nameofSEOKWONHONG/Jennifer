using Jennifer.Tenant.Models;

namespace Jennifer.Tenant.Extensions;

public class ITenantSessionContext
{
    public string TenantId { get; set; }
    public Guid TenantGuid => Guid.Parse(TenantId);
}

public static class QueryFilters
{
    public static IQueryable<T> SetTenant<T>(this IQueryable<T> query, ITenantSessionContext session)
        where T : ITenantEntity
    {
        return query.Where(m => m.TenantId == session.TenantGuid);
    }
}