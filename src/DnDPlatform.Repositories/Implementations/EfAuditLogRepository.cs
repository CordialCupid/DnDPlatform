using DnDPlatform.Models.Domain;
using DnDPlatform.Repositories.Data;
using DnDPlatform.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DnDPlatform.Repositories.Implementations;

public class EfAuditLogRepository(DnDDbContext db) : IAuditLogRepository
{
    public async Task InsertAsync(AuditLog entry)
    {
        db.AuditLogs.Add(entry);
        await db.SaveChangesAsync();
    }

    public Task<IEnumerable<AuditLog>> GetByResourceAsync(Guid resourceId, int limit = 50)
    {
        return Task.FromResult<IEnumerable<AuditLog>>( db.AuditLogs
                .Where(a => a.ResourceId == resourceId)
                .OrderByDescending(a => a.Timestamp)
                .Take(limit)
                .AsEnumerable());
    }

    public Task<IEnumerable<AuditLog>> GetByUserAsync(Guid userId, int limit = 50)
    {
        return Task.FromResult<IEnumerable<AuditLog>>( db.AuditLogs
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Timestamp)
                .Take(limit)
                .AsEnumerable());   
    }
}
