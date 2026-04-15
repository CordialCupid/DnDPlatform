namespace DnDPlatform.Models.Domain;

public class CharacterSheet
{
    public Guid Id { get; set; }
    public Guid CharacterId { get; set; }
    public Character Character { get; set; } = null!;
    public int VersionNumber { get; set; }
    public string Label { get; set; } = string.Empty;
    public string JsonBlob { get; set; } = "{}";
    public bool IsSnapshot { get; set; }
    public DateTime CreatedAt { get; set; }
}
