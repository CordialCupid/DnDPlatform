using DnDPlatform.Models.Enums;

namespace DnDPlatform.Services.Interfaces;

public interface IAuditLogService
{
    Task LogAsync(Guid userId, string username, AuditAction action, ResourceType resourceType, Guid resourceId, object? metadata = null);
}
