using GymProject.Models;
using GymProject.Repositories.Interfaces;
using GymProject.Services.Interface;
using System.Text.Json;

namespace GymProject.Services.Implementation
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly ILogger<AuditLogService> _logger;

        public AuditLogService(IAuditLogRepository auditLogRepository, ILogger<AuditLogService> logger)
        {
            _auditLogRepository = auditLogRepository;
            _logger = logger;
        }

        public async Task LogActivityAsync<T>(string? userId, string type, string tableName, string primaryKey, T? oldValues, T? newValues) where T : class
        {
            try
            {
                var affectedColumns = new List<string>();

                // If it's an Update, find which properties changed
                if (type == "Update" && oldValues != null && newValues != null)
                {
                    var properties = typeof(T).GetProperties();
                    foreach (var prop in properties)
                    {
                        var oldValue = prop.GetValue(oldValues);
                        var newValue = prop.GetValue(newValues);

                        if (!Equals(oldValue, newValue))
                        {
                            affectedColumns.Add(prop.Name);
                        }
                    }
                }

                // Create the audit log entry
                var log = new AuditLog
                {
                    UserId = userId,
                    Type = type,
                    TableName = tableName,
                    DateTime = DateTime.UtcNow,
                    PrimaryKey = primaryKey,
                    OldValues = oldValues != null ? JsonSerializer.Serialize(oldValues) : null,
                    NewValues = newValues != null ? JsonSerializer.Serialize(newValues) : null,
                    AffectedColumns = affectedColumns.Count > 0 ? JsonSerializer.Serialize(affectedColumns) : null
                };

                await _auditLogRepository.InsertLogAsync(log);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log activity for table: {TableName}", tableName);
                // We generally do NOT throw here because we don't want audit log failures to break the main application flow.
            }
        }
    }
}
