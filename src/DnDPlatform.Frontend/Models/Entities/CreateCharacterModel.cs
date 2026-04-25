using System.ComponentModel.DataAnnotations;

namespace DnDPlatform.Frontend.Models.Entities;

public class CreateCharacterModel
{
    [Required]
    public string CharacterName {get;set;} = String.Empty; 
    [Required]
    public string TemplateId {get;set;} = String.Empty;
}
