using DnDPlatform.Models.Enums;

namespace DnDPlatform.Models.Domain;

public class AuditLog
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public AuditAction Action { get; set; }
    public ResourceType ResourceType { get; set; }
    public Guid ResourceId { get; set; }
    public string MetadataJson { get; set; } = "{}";
    public DateTime Timestamp { get; set; }
}
