using System.ComponentModel.DataAnnotations;
namespace DnDPlatform.Frontend.Models.FormModels;
public class TemplateModel
{
    [Required, MinLength(1), MaxLength(100)]
    public string Name { get; set; } = String.Empty;
    [Required]
    public string System { get; set; } = String.Empty;
    [Required]
    public string Fields {get;set;} = String.Empty;
}