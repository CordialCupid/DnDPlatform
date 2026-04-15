namespace DnDPlatform.Models.DTOs.Characters;

public class CharacterDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Class { get; set; } = string.Empty;
    public string Backstory { get; set; } = string.Empty;
    public int Level { get; set; }
    public Guid TemplateId { get; set; }
    public string TemplateName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public SheetVersionDto? CurrentSheet { get; set; }
}
