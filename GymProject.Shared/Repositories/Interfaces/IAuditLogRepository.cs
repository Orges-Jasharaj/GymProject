using GymProject.Models;

namespace GymProject.Repositories.Interfaces
{
    public interface IAuditLogRepository
    {
        Task<bool> InsertLogAsync(AuditLog log);
    }
}
