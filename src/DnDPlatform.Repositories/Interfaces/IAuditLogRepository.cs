using DnDPlatform.Models.Domain;

namespace DnDPlatform.Repositories.Interfaces;

public interface IAuditLogRepository
{
    Task InsertAsync(AuditLog entry);
    Task<IEnumerable<AuditLog>> GetByResourceAsync(Guid resourceId, int limit = 50);
    Task<IEnumerable<AuditLog>> GetByUserAsync(Guid userId, int limit = 50);
}
