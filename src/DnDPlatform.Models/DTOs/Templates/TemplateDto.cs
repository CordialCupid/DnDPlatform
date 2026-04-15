namespace DnDPlatform.Models.DTOs.Templates;

public class TemplateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string System { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string JsonSchema { get; set; } = "{}";
    public string DefaultLayoutJson { get; set; } = "{}";
    public Guid OwnerId { get; set; }
    public DateTime CreatedAt { get; set; }
}
