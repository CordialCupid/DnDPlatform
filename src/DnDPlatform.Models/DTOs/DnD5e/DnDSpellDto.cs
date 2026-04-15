namespace DnDPlatform.Models.DTOs.DnD5e;

public class DnDSpellDto
{
    public string Index { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Level { get; set; }
    public string School { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}
