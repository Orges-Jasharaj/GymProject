using Dapper;
using GymProject.Models;
using GymProject.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GymProject.Repositories.Implemntations
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuditLogRepository> _logger;

        public AuditLogRepository(IConfiguration configuration, ILogger<AuditLogRepository> logger)
        {
            _configuration = configuration;
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
                
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
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
