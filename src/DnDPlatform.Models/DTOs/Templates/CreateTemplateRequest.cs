using System.ComponentModel.DataAnnotations;

namespace DnDPlatform.Models.DTOs.Templates;

public class CreateTemplateRequest
{
    [Required, MinLength(1), MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string System { get; set; } = string.Empty;

    public string Type { get; set; } = "character";

    [Required]
    public string JsonSchema { get; set; } = "{}";

    public string DefaultLayoutJson { get; set; } = "{}";
}
