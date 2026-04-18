using Dapper;
using GymProject.Data;
using GymProject.Models;
using GymProject.Repositories.Interfaces;

namespace GymProject.Repositories.Implemntations
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly DapperContext _context;
        private readonly ILogger<AuditLogRepository> _logger;

        public AuditLogRepository(DapperContext context, ILogger<AuditLogRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> InsertLogAsync(AuditLog log)
        {
            try
            {
                var query = @"INSERT INTO AuditLogs
                            (UserId, Type, TableName, DateTime, OldValues, NewValues, AffectedColumns, PrimaryKey)
                            VALUES
                            (@UserId, @Type, @TableName, @DateTime, @OldValues, @NewValues, @AffectedColumns, @PrimaryKey)";
                
                using var connection = _context.CreateConnection();
                return await connection.ExecuteAsync(query, log) > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inserting AuditLog for Table: {TableName}", log.TableName);
                return false;
            }
        }
    }
}
