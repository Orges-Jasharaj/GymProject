using GymProject.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;

namespace GymProject.Data
{
    public class AuditEntry
    {
        public EntityEntry Entry { get; }
        public string? UserId { get; set; }
        public string TableName { get; set; } = null!;
        public Dictionary<string, object?> KeyValues { get; } = new Dictionary<string, object?>();
        public Dictionary<string, object?> OldValues { get; } = new Dictionary<string, object?>();
        public Dictionary<string, object?> NewValues { get; } = new Dictionary<string, object?>();
        public List<string> ChangedColumns { get; } = new List<string>();
        public string AuditType { get; set; } = null!;

        public AuditEntry(EntityEntry entry)
        {
            Entry = entry;
        }

        public AuditLog ToAudit()
        {
            var audit = new AuditLog();
            audit.UserId = UserId;
            audit.Type = AuditType;
            audit.TableName = TableName;
            audit.DateTime = DateTime.UtcNow;
            audit.PrimaryKey = JsonSerializer.Serialize(KeyValues);
            audit.OldValues = OldValues.Count == 0 ? null : JsonSerializer.Serialize(OldValues);
            audit.NewValues = NewValues.Count == 0 ? null : JsonSerializer.Serialize(NewValues);
            audit.AffectedColumns = ChangedColumns.Count == 0 ? null : JsonSerializer.Serialize(ChangedColumns);
            return audit;
        }
    }
}
