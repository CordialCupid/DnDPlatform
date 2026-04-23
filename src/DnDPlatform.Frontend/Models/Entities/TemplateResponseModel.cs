using System.ComponentModel.DataAnnotations;
namespace DnDPlatform.Frontend.Models.Entities;
public class TemplateResponseModel
{
    public Guid id { get; set; }
    [Required, MinLength(1), MaxLength(100)]
    public string name { get; set; } = string.Empty;
    [Required]
    public string system { get; set; } = string.Empty;
    public string type { get; set; } = string.Empty;
    public string jsonSchema { get; set; } = "{}";
    public string defaultLayoutJson { get; set; } = "{}";
    public Guid ownerId { get; set; }
    public DateTime createdAt { get; set; }
}