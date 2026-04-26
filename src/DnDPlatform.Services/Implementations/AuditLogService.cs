using DnDPlatform.Models.Domain;
using DnDPlatform.Models.Enums;
using DnDPlatform.Repositories.Interfaces;
using DnDPlatform.Services.Interfaces;
using System.Text.Json;

namespace DnDPlatform.Services.Implementations;

public class AuditLogService : IAuditLogService
{
    private readonly IAuditLogRepository _auditRepo;

    public AuditLogService(IAuditLogRepository auditRepo)
    {
        _auditRepo = auditRepo;
    }
    public Task LogAsync(Guid userId, string username, AuditAction action, ResourceType resourceType, Guid resourceId, object? metadata = null)
    {
        var entry = new AuditLog
        {
            UserId = userId,
            Username = username,
            Action = action,
            ResourceType = resourceType,
            ResourceId = resourceId,
            MetadataJson = metadata is null ? "{}" : JsonSerializer.Serialize(metadata),
            Timestamp = DateTime.UtcNow
        };
        return _auditRepo.InsertAsync(entry);
    }
}
