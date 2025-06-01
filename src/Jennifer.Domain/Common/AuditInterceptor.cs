using eXtensionSharp;
using Jennifer.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Jennifer.Domain.Common;

public class AuditInterceptor : SaveChangesInterceptor
{
    private readonly List<AuditEntry> _auditEntries;
    
    public AuditInterceptor()
    {
        _auditEntries = new();
    }
    
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        var auditEntries = new List<AuditEntry>();
        foreach (var entry in eventData.Context.ChangeTracker.Entries())
        {
            if (entry.Entity is not IAuditable entity)
                continue;
            
            switch (entry.State)
            {
                case EntityState.Detached:
                case EntityState.Unchanged:
                    continue;
            }

            var auditEntry = new AuditEntry(entry)
            {
                TableName = entity.GetType().Name,
                UserId = entity.CreatedBy
            };
            auditEntries.Add(auditEntry);
            foreach (var property in entry.Properties)
            {
                if (property.IsTemporary)
                {
                    auditEntry.TemporaryProperties.Add(property);
                    continue;
                }

                string propertyName = property.Metadata.Name;
                if (property.Metadata.IsPrimaryKey())
                {
                    auditEntry.KeyValues[propertyName] = property.CurrentValue;
                    continue;
                }

                switch (entry.State)
                {
                    case EntityState.Added:
                        auditEntry.AuditType = "create";
                        auditEntry.NewValues[propertyName] = property.CurrentValue;
                        break;

                    case EntityState.Deleted:
                        auditEntry.AuditType = "delete";
                        auditEntry.OldValues[propertyName] = property.OriginalValue;
                        break;

                    case EntityState.Modified:
                        if (property.IsModified && property.OriginalValue?.Equals(property.CurrentValue) == false)
                        {
                            auditEntry.ChangedColumns.Add(propertyName);
                            auditEntry.AuditType = "update";
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                        }
                        break;
                }
            }
        }

        var auditItems = auditEntries
            .Where(m => !m.HasTemporaryProperties && !string.IsNullOrWhiteSpace(m.AuditType))
            .ToArray();
        
        if (auditItems is {Length: > 0})
        {
            _auditEntries.AddRange(auditItems);    
        }
        
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        if (_auditEntries is {Count: > 0})
        {
            eventData.Context.Set<Audit>().AddRange(_auditEntries.Select(m => m.ToAudit()));
            _auditEntries.Clear();
            await eventData.Context.SaveChangesAsync(cancellationToken);
        }

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }
}