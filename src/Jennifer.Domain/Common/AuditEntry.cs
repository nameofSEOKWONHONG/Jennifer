using System.Text.Json;
using eXtensionSharp;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Jennifer.Domain.Common;

internal class AuditEntry
{
    public AuditEntry(EntityEntry entry)
    {
        Entry = entry;
    }

    public EntityEntry Entry { get; }
    public string UserId { get; set; }
    public string TableName { get; set; }
    public Dictionary<string, object> KeyValues { get; } = new();
    public Dictionary<string, object> OldValues { get; } = new();
    public Dictionary<string, object> NewValues { get; } = new();
    public List<PropertyEntry> TemporaryProperties { get; } = new();
    public string AuditType { get; set; }
    
    public List<string> ChangedColumns { get; } = new();
    
    public bool HasTemporaryProperties => TemporaryProperties.Any();

    public Audit ToAudit()
    {
        var audit = new Audit
        {
            UserId = UserId,
            Type = AuditType,
            TableName = TableName,
            DateTime = DateTime.UtcNow,
            PrimaryKey = JsonSerializer.Serialize(KeyValues),
            OldValues = OldValues.xIsEmpty() ? null : JsonSerializer.Serialize(OldValues),
            NewValues = NewValues.xIsEmpty() ? null : JsonSerializer.Serialize(NewValues),
            AffectedColumns = ChangedColumns.xIsEmpty() ? null : JsonSerializer.Serialize(ChangedColumns)
        };
        return audit;
    }
}