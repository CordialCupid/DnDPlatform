namespace DnDPlatform.Models.DTOs.Characters;

public class SheetVersionDto
{
    public Guid Id { get; set; }
    public Guid CharacterId { get; set; }
    public int VersionNumber { get; set; }
    public string Label { get; set; } = string.Empty;
    public string JsonBlob { get; set; } = "{}";
    public bool IsSnapshot { get; set; }
    public DateTime CreatedAt { get; set; }
}
