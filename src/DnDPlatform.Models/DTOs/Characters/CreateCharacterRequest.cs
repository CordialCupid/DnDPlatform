using System.ComponentModel.DataAnnotations;

namespace DnDPlatform.Models.DTOs.Characters;

public class CreateCharacterRequest
{
    [Required]
    public Guid TemplateId { get; set; }

    [Required, MinLength(1), MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Class { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string Backstory { get; set; } = string.Empty;
}
