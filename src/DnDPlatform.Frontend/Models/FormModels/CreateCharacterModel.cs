using System.ComponentModel.DataAnnotations;

namespace DnDPlatform.Frontend.Models.FormModels;

public class CreateCharacterModel
{
    [Required]
    public string CharacterName {get;set;} = String.Empty; 
    [Required]
    public string Class {get;set;} = String.Empty;
    [Required]
    public string BackStory {get;set;} = String.Empty;
    [Required]
    public int Level {get;set;}
    [Required]
    public string TemplateId {get;set;} = String.Empty;
}
