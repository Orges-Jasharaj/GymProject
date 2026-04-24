namespace GymProject.Services.Interface
{
    public interface IAuditLogService
    {
        Task LogActivityAsync<T>(string? userId, string type, string tableName, string primaryKey, T? oldValues, T? newValues) where T : class;
    }
}
