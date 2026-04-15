namespace DnDPlatform.Models.DTOs.Characters;

public class VersionSummaryDto
{
    public Guid Id { get; set; }
    public int VersionNumber { get; set; }
    public string Label { get; set; } = string.Empty;
    public bool IsSnapshot { get; set; }
    public DateTime CreatedAt { get; set; }
}
