namespace DnDPlatform.Models.DTOs.DnD5e;

public class DnDAbilityScoreDto
{
    public string Index { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public List<string> Description { get; set; } = [];
    public string Url { get; set; } = string.Empty;
}
